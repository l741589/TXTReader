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

    function update($table, $data, $cond, $format = null, $cond_format = null) {

    }

    function delete($table, $cond, $cond_format = null) {

    }

    /**
     *
     */
    function get_val() {

    }

    /**
     *
     */
    function get_row() {

    }

    /**
     *
     */
    function get_col() {

    }

    /**
     *
     */
    function get_result() {

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

