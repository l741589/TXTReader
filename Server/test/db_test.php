<?php
/**
 * Created by JetBrains PhpStorm.
 * User: van
 * Date: 13-10-24
 * Time: 下午6:30
 * To change this template use File | Settings | File Templates.
 */

require __DIR__ . '/../src/db.php';
require_once "PHPUnit/Extensions/Database/TestCase.php";

class db_test extends PHPUnit_Framework_TestCase {
    /**
     * @var db
     */
    protected  $db;

    protected function setUp() {
        // databae arguments for need to be specified 
        $this->db = new db("127.0.0.1:3307", "txtreader", "root", "");
    }

    public function test_db_connection() {
        $this->assertNotEmpty($this->db);
    }

    public function test_func_prepare() {
        $db = $this->db;
        $sql = $db->prepare("select * from aaa where bbb = %s and ccc = %d", 'foo', 1337);
        $this->assertStringEndsNotWith("select * from aaa where bbb = foo and ccc = 1337", $sql);
    }

    public function test_func_query() {
        $sql = "insert into txtreader.users(username, password) values ('a', '1')";
        $this->db->db_connect();
        $ret_val = $this->db->query($sql);
        $this->assertNotEquals(0, $ret_val);
        $this->assertEquals(1, $ret_val);
        $sql = "select * from txtreader.users where username = 'a'";
        $this->db->db_connect();
        $ret_val = $this->db->query($sql);
        $this->assertNotEquals(0, $ret_val);
        $sql = "delete from txtreader.users where username = 'a'";
        $this->db->db_connect();
        $ret_val = $this->db->query($sql);
        $this->assertNotEquals(0, $ret_val);
    }

    public function test_insert() {
        $this->db->db_connect();
        $ret_val = $this->db->insert("users", array("username"=>"ccc", "password"=>"111"));
        $this->assertEquals(1, $ret_val);
        $this->db->db_connect();
        $ret_val = $this->db->insert("users", array("username"=>"ccc", "password"=>"111"), array("%s", "%s"));
        $this->assertEquals(1, $ret_val);
        $this->db->db_connect();
        $ret_val = $this->db->insert("users", array("username"=>"ccc", "password"=>"111"), "%s, %s");
        $this->assertEquals(1, $ret_val);
    }
}
