<?php
/**
 * Created by PhpStorm.
 * User: van
 * Date: 13-10-26
 * Time: 下午11:51
 */

require __DIR__ . "/../src/db.php";
require __DIR__ . "/../src/config.php";
require __DIR__ . "/../src/upload.php";


class upload_test extends PHPUnit_Framework_TestCase {

    protected $id;
    /**
     * @var db $db
     */
    protected $db;

    protected function setUp() {
        $this->db = new db(DB_HOST, DB_NAME, DB_USER, DB_PASSWORD);
        $this->db->insert("users", array("username"=>"ddd", "password"=>"123456"));
        $this->db->insert("users", array("username"=>"bbb", "password"=>"111"));
        $this->id = $this->db->inserted_id;
    }

    protected function tearDown() {
        $this->db->delete("users", array("1"=>"1"));
        $this->db->delete("books", array("1"=>"1"));
    }

    public function test_get_user_id() {
        $ret_id = get_user_id("ddd", "123456");
        $this->assertNotNull($ret_id);
        $this->assertNotEquals(0, $ret_id);
        $ret_id = get_user_id("bbb", "111' and '1' = ' ");
        $this->assertNull($ret_id);
    }

    // This test must be in real server environment.
    // In this test, I use Apache as server and set
    // Server/src as DocumentRoot
    public function test_upload_file() {
        $target_url = "http://localhost:9111/src/upload.php";
        // test1.txt is just a file
        $file_with_full_path = realpath("../_files/test1.txt");
        $post = array("username"=>"ddd", "password"=>"123456", "file"=>'@'.$file_with_full_path);
        $ch = curl_init();
        curl_setopt($ch, CURLOPT_URL, $target_url);
        curl_setopt($ch, CURLOPT_POST, 1);
        curl_setopt($ch, CURLOPT_POSTFIELDS, $post);
        $result=curl_exec ($ch);
        curl_close ($ch);
        $this->assertEquals(1, $result);
    }
}
?>