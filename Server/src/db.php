<?php
/**
 * Created by JetBrains PhpStorm.
 * User: van
 * Date: 13-10-18
 * Time: 下午7:58
 * To change this template use File | Settings | File Templates.
 */

class db {
    protected $dbhost;
    protected $dbname;
    protected $dbuser;
    protected $dbpassword;
    protected $dbconn;
    var $tables = array("users", "books");
    var $ready = false;
    var $last_query;
    var $result;
    var $last_result;
    var $last_error;
    var $rows_affected;
    var $num_rows;
    var $inserted_id;

    function __construct($dbhost,$dbname, $dbuser, $dbpassword) {
        register_shutdown_function(array($this, '__destruct'));
        $this->dbuser = $dbuser;
        $this->dbpassword = $dbpassword;
        $this->dbname = $dbname;
        $this->dbhost = $dbhost;

        $this->db_connect();
    }

    function __destruct() {
        return true;
    }

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

    function db_select($dbname, $dbconn = null) {
        if (is_null($dbconn)) {
            $dbconn = $this->dbconn;
        }

        if (!mysql_select_db($dbname, $dbconn)) {
            $this->ready = false;
        }
    }

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

    function insert($table, $data, $format = null) {
        $this->inserted_id = 0;
        $format = (array)$format;
        $fields = array_keys($data);
    }


    function update() {

    }

    function delete() {

    }

    function get_val() {

    }

    function get_row() {

    }

    function get_cow() {

    }

    function get_result() {

    }

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

    function flush() {
        $this->last_result = array();
        $this->last_query = null;
        $this->rows_affected = $this->num_rows = 0;
        $this->last_error  = '';

        if ( is_resource($this->result))
            mysql_free_result($this->result);
    }

    function escape(&$string) {
        if (!is_float($string))
            addslashes($string);
    }

    function print_error() {

    }
}

function connect() {
    $conn = mysql_connect("127.0.0.1:3307", "root", "313633893") or die("Error: Could not connect to database");
    mysql_select_db("txtreader", $conn) or die ("Error: Could not select database");
}

function close() {
    mysql_close();
}

