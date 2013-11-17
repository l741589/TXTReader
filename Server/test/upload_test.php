<?php
/**
 * Created by PhpStorm.
 * User: van
 * Date: 13-10-26
 * Time: 下午11:51
 */

require __DIR__ . "/../src/upload.php";


class upload_test extends PHPUnit_Framework_TestCase {

    protected $id;
    protected $db;

    protected function setUp() {
        $this->db = new db(DB_HOST, "txtreader", DB_USER, DB_PASSWORD);
        $this->db->insert("users", array("username"=>"ddd", "password"=>"123456"));
        $this->db->insert("users", array("username"=>"bbb", "password"=>"111"));
        $this->id = $this->db->inserted_id;
    }

    protected function tearDown() {
        // $this->db->delete("users", array("1"=>"1"));
    }

    public function test_get_user_id() {
        $ret_id = get_user_id("ddd", "123456");
        $this->assertNotNull($ret_id);
        $this->assertNotEquals(0, $ret_id);
        $ret_id = get_user_id("bbb", "111' and '1' = ' ");
        $this->assertNull($ret_id);
    }

    public function test_upload_file() {
        curl_exec();
        $test_file_path = __DIR__ . "/../_files/test1.txt";
        $size = filesize($test_file_path);
        $_FILES = array(
            'test' => array(
            "name"=>"test1.txt",
            'tmp_name'=>$test_file_path,
            'type'=>'text/plain',
            'size'=>$size,
            'error'=>0
        ));
        $user_id = $this->id;

        $ret = save_file($_FILES["test"], $user_id);
        $this->assertEquals(true, $ret);
    }
}

