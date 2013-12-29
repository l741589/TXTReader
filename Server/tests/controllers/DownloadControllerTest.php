<?php if (!defined('BASEPATH')) exit('No direct script access allowed');
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-11-24
 * Time: 下午6:38
 */

class DownloadControllerTest extends CIUnit_TestCase
{

    private $_book_model;

    public static function setUpBeforeClass()
    {
        $conn = new mysqli("localhost:3306", "root", "123456", "txtreader");
        $conn->autocommit(false);
        $conn->query("DELETE FROM book");
        $conn->query("DELETE FROM file");
        if (!$conn->errno) {
            $conn->commit();
            echo("Database is ready");
        } else {
            $conn->rollback();
            echo("Database is not ready");
        }
    }

    public function setUp()
    {
        $this->CI = set_controller("download_controller");
        $this->_book_model = $this->CI->load->model('Book_Model');
        $this->CI->add_session('testUserForDownload');
    }

    public function tearDown()
    {
        $this->CI->del_session();
    }

    public function testDownload()
    {
        $dir = dirname(__DIR__);
        $realFileDir = $dir . "/testfiles/test1.txt";
        $this->_file_info = array(
            "user_id"   => 1,
            "book_name" => "file.txt",
            "file_md5"  => md5_file($realFileDir),
            "file_data" => mysql_real_escape_string(file_get_contents($realFileDir))
        );
        $this->CI->Book_Model->save_book($this->_file_info);
        $_GET['id'] = '';
        $ret = $this->CI->do_download();
        $this->assertEquals(RESULT_MISSING_ARGS, $ret);
        $_GET['id'] = $this->CI->Book_Model->inserted_book_id() + 1;
        $ret = $this->CI->do_download();
        $this->assertEquals(RESULT_NO_BOOK, $ret);
        $_GET['id'] = $this->CI->Book_Model->inserted_book_id();
        $ret = $this->CI->do_download();
        $this->assertEquals(RESULT_SUCCESS, $ret);
    }
}