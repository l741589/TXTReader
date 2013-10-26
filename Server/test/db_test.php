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
        $this->db = new db("127.0.0.1:3307", "txtreader", "root", "313633893");
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
        $ret_val = $this->db->insert("users", array("username"=>"bbb", "password"=>"111"), array("%s", "%s"));
        $this->assertEquals(1, $ret_val);
    }

    public function test_update() {
        $this->db->db_connect();
        $ret_val = $this->db->update("users", array("password" => "123456"), array("username"=>"ccc", "password"=>"111"));
        $this->assertEquals(1, $ret_val);
        $this->db->db_connect();
        $ret_val = $this->db->update("users", array("password" => "222"), array("username"=>"bbb"), array("%s"));
        $this->assertEquals(1, $ret_val);
    }

    public function test_delete() {
        $this->db->db_connect();
        $ret_val = $this->db->delete("users", array("username"=>"ccc", "password"=>"123456"));
        $this->assertEquals(1, $ret_val);
    }

    // clear all the test data
    public function test_db_clear() {
        $this->db->delete("users", array("1" => 1));
    }
}
