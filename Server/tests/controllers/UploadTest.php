<?php if (!defined('BASEPATH')) exit('No direct script access allowed');
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-11-24
 * Time: 下午6:38
 */

class UploadTest extends CIUnit_TestCase {

    public static function setUpBeforeClass() {
        $_test_user = array(
            'username' => 'testUserForUpload',
            'password' => 'testPassword'
        );
        $conn = new mysqli("localhost:3306", "root", "123456", "txtreader");
        $conn->autocommit(false);
        $conn->query("DELETE FROM user");
        $conn->query("DELETE FROM book");
        $conn->query("DELETE FROM file");
        $conn->query("DELETE FROM user_book_relation");
        $query = "insert into user(username, password) values ('"
            . $_test_user['username'] . "', '" . sha1($_test_user['password']) . "')";
        $conn->query($query);
        if (!$conn->errno) {
            $conn->commit();
            echo("Database is ready");
        } else {
            $conn->rollback();
            echo("Database is not ready");
        }
    }

    public function setUp() {
        $this->CI = set_controller("book_controller");
        $this->CI->add_session('testUserForUpload');
    }

    public function tearDown() {
        $this->CI->del_session();
    }

    public function testDoUpload() {
        $dir = dirname(__DIR__);
        $realFilePath = $dir . "/testfiles/test1.txt";
//        $ret = $this->_upload($realFilePath, "test1.txt");
        $_FILES = array(
            'userfile' => array(
                'name' => "test1.txt",

            )
        );
        //$this->assertEquals(true, $ret);
    }

//    public function _upload($realFilePath, $fileName) {
//        $post_data = array(
//            "userfile" => '@' . $realFilePath
//        );
//        $test_user = array(
//            'username' => 'testUserForUpload',
//            'password' => 'testPassword'
//        );
//        $args['userfile'] = new CurlFile($realFilePath, '“text/plain', $fileName);
//        $curl = curl_init();
//        curl_setopt($curl, CURLOPT_URL, "http://localhost:9999/login");
//        curl_setopt($curl, CURLOPT_POST, 1);
//        curl_setopt($curl, CURLOPT_POSTFIELDS, $test_user);
//        curl_exec($curl);
//        curl_setopt($curl, CURLOPT_URL, "http://localhost:9999/upload");
//        curl_setopt($curl, CURLOPT_POST, 1);
//        curl_setopt($curl, CURLOPT_POSTFIELDS, $args);
//        curl_setopt($curl, CURLOPT_RETURNTRANSFER, 1);
//        $ret = curl_exec($curl);
//        curl_close($curl);
//        return $ret;
//    }
}