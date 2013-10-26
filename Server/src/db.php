<?
/**
 * Created by JetBrains PhpStorm.
 * User: van
 * Date: 13-10-18
 * Time: 下午7:58
 * To change this template use File | Settings | File Templates.
 */

class db {
    /**
     * @var
     */
    protected $dbhost;
    /**
     * @var
     */
    protected $dbname;
    /**
     * @var
     */
    protected $dbuser;
    /**
     * @var
     */
    protected $dbpassword;
    /**
     * @var
     */
    protected $dbconn;
    /**
     * @var array
     */
    var $tables = array("users", "books");
    /**
     * @var bool
     */
    var $ready = false;
    /**
     * @var
     */
    var $last_query;
    /**
     * @var
     */
    var $result;
    /**
     * @var
     */
    var $last_result;
    /**
     * @var
     */
    var $last_error;
    /**
     * @var
     */
    var $rows_affected;
    /**
     * @var
     */
    var $num_rows;
    /**
     * @var
     */
    var $inserted_id;

    /**
     * @param $dbhost
     * @param $dbname
     * @param $dbuser
     * @param $dbpassword
     */
    function __construct($dbhost,$dbname, $dbuser, $dbpassword) {
        register_shutdown_function(array($this, '__destruct'));
        $this->dbuser = $dbuser;
        $this->dbpassword = $dbpassword;
        $this->dbname = $dbname;
        $this->dbhost = $dbhost;

        $this->db_connect();
    }

    /**
     *
     */
    function __destruct() {
        return true;
    }

    /**
     *
     */
    function db_connect() {
        $this->dbconn = mysql_connect($this->dbhost, $this->dbuser, $this->dbpassword);
        if (!$this->dbconn)
        {
            error_log("Error: Cannot connect to database.");
            return ;
        }
        $this->ready = true;
        $this->db_select($this->dbname, $this->dbconn);
    }

    /**
     * @param $dbname
     * @param null $dbconn
     */
    function db_select($dbname, $dbconn = null) {
        if (is_null($dbconn)) {
            $dbconn = $this->dbconn;
        }

        if (!mysql_select_db($dbname, $dbconn)) {
            $this->ready = false;
        }
    }

    /**
     * @param $query
     * @return bool|int|resource
     */
    function query($query) {
        if (!$this->ready)
            return false;
        // clear all the data
        $this->flush();
        $this->last_query = $query;
        $this->result = @mysql_query($query, $this->dbconn);
        if ($this->last_error = mysql_error($this->dbconn)) {
            if ( $this->insert_id && preg_match( '/^\s*(insert|replace)\s/i', $query ) ) {
                $this->insert_id = 0;
            }
            $this->rows_affected = 0;
            return false;
        }

        $return_val = 0;

        if (preg_match('/^\s*(create|alter|truncate|drop)\s/i', $query)) {
            $return_val = $this->result;
        } elseif (preg_match('/^\s*(insert|delete|update)\s/i', $query)) {
            $this->rows_affected  = mysql_affected_rows($this->dbconn);
            // record last inserted result id
            if (preg_match('/\s*(insert)\s/i', $query)) {
                $this->inserted_id = mysql_insert_id($this->dbconn);
            }
            // return number of rows affected
            $return_val = $this->rows_affected;
        } else {
            $num_rows = 0;
            while($row = @mysql_fetch_object($this->result)) {
                $this->last_result[$num_rows] = $row;
                $num_rows++;
            }
            $this->num_rows = $num_rows;
            $return_val = $num_rows;
        }
        return $return_val;
    }

    /**
     * @param $table
     * @param $data
     * @param null $format
     */
    function insert($table, $data, $format = null) {
        $this->inserted_id = 0;
        $formats = $format = (array)$format;
        $fields = array_keys($data);
        $fields_formatted = array();
        foreach ($fields as $field) {
            if (!empty($format)) {
                $form = ($form = array_shift($formats)) ? $form : null;
            }
            else {
                $form = '%s';
            }
            if (!is_null($form))
                $fields_formatted[] = $form;
        }
        $sql = "insert into $table (".implode(',', $fields).") values (".implode(",", $fields_formatted).")";
        return $this->query($this->prepare($sql, $data));
    }

    /**
     * @param $table
     * @param $data
     * @param $cond
     * @param null $format
     * @param null $cond_format
     * @return bool|int|resource
     */
    function update($table, $data, $cond, $format = null, $cond_format = null) {
        if (!is_array($data) && !is_array($cond)) {
            return false;
        }

        $formats = $format = (array) $format;
        $cond_formats = $cond_format = (array) $cond_format;
        $fields_formatted = $conds_formatted = array();
        foreach(array_keys($data) as $field) {
            if (!empty($format)) {
                $form = ($form = array_shift($formats)) ? $form : null;
            } else {
                $form = '%s';
            }
            if (!is_null($form)) {
                $fields_formatted[] = "$field = {$form}";
            }
        }
        foreach(array_keys($cond) as $field) {
            if (!empty($cond_format)) {
                $form = ($form = array_shift($cond_formats)) ? $form : null;
            } else {
                $form = '%s';
            }
            if (!is_null($form)) {
                $conds_formatted[] = "$field = {$form}";
            }
        }

        $sql = "update $table set " . implode(',', $fields_formatted) . " where " . implode(' and ', $conds_formatted);
        return $this->query($this->prepare($sql, array_merge(array_values($data), array_values($cond))));
    }

    /**
     * @param $table
     * @param $cond
     * @param null $cond_format
     * @return bool|int|resource
     */
    function delete($table, $cond, $cond_format = null) {

        if (!is_array($cond))
            return false;

        $cond_formats = $cond_format = (array) $cond_format;
        $cond_formatted = array();
        foreach(array_keys($cond) as $fields) {
            if (!empty($cond_format)) {
                $form = ($form = array_shift($cond_formats)) ? $form : null;
            } else {
                $form = '%s';
            }
            if (!is_null($form)) {
                $cond_formatted[] = "$fields = {$form}";
            }
        }

        $sql = "delete from $table where " . implode(' and ', $cond_formatted);
        return $this->query($this->prepare($sql, $cond));
    }

    /**
     * @param $query
     * @param $x
     * @param $y
     * @return null
     */
    function get_val($query, $x = 0, $y = 0) {
        if ($query) {
            $this->query($query);
        }

        if (!empty($this->last_result[$y])) {
            $values = array_values(get_object_vars($this->last_result[$y]));
        }

        return (isset($values[$x]) && $values[$x] != '') ? $values[$x] : null;
    }

    /**
     * @param $query
     * @param int $y
     * @return array|null
     */
    function get_row($query, $y = 0) {
        if ($query) {
            $this->query($query);
        }

        if (!isset($this->last_result[$y]))
            return null;

        return get_object_vars($this->last_result[$y]);
    }

    /**
     * @param $query
     * @param int $x
     * @return array
     */
    function get_col($query, $x = 0) {
        if ($query)
            $this->query($query);
        $ret_arr = array();
        for ($i = 0; $i < count($this->last_result); ++$i) {
            $ret_arr[$i] = $this->get_val(null, $x, $i);
        }
        return $ret_arr;
    }

    /**
     *
     */
    function get_result($query) {
        if ($query)
            $this->query($query);
        else
            return null;

        $ret = array();
        foreach($this->last_result as $row) {
            $var = get_object_vars($row);
            $key = array_shift($var);
            if (!isset($ret)) {
                $ret[$key] = $row;
            }
        }
        return $ret;
    }

    /**
     * @param $query
     * @param $args
     * @return string
     */
    public function prepare($query, $args) {
        if (is_null($query))
            return ;

        $args = func_get_args();
        array_shift($args);
        if (is_array($args[0]))
            $args = $args[0];
        $query = str_replace("'%s'", '%s', $query); // in case someone mistakenly already singlequoted it
        $query = str_replace('"%s"', '%s', $query); // doublequote unquoting
        $query = preg_replace('|(?<!%)%f|' , '%F', $query); // Force floats to be locale unaware
        $query = preg_replace('|(?<!%)%s|', "'%s'", $query); // quote the strings, avoiding escaped strings like %%s
        array_walk($args, array( $this, 'escape'));
        return @vsprintf($query, $args);
    }

    /**
     *
     */
    function flush() {
        $this->last_result = array();
        $this->last_query = null;
        $this->rows_affected = $this->num_rows = 0;
        $this->last_error  = '';

        if ( is_resource($this->result))
            mysql_free_result($this->result);
    }

    /**
     * @param $string
     */
    function escape(&$string) {
        if (!is_float($string))
            addslashes($string);
    }

    /**
     * @param $error
     */
    function print_error($error) {
        return ;
    }
}

