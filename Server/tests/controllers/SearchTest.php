<?php if (!defined('BASEPATH')) exit('No direct script access allowed');
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-11-24
 * Time: 下午6:38
 */

class SearchTest extends CIUnit_TestCase {

    private $_book_model;
    public static $user_id;

    public static function setUpBeforeClass()
    {
        $_test_user = array(
            'username' => 'testUserForSearch',
            'password' => 'testPassword'
        );
        $conn = new mysqli("localhost:3306", "root", "123456", "txtreader");
        $conn->autocommit(false);
        $conn->query("DELETE FROM book");
        $conn->query("DELETE FROM user");
        $conn->query("DELETE FROM user_book_relation");
        $query = "insert into user(username, password) values ('"
            . $_test_user['username'] . "', '" . sha1($_test_user['password']) . "')";
        $conn->query($query);
        self::$user_id = $conn->insert_id;
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
        $this->CI = set_controller("book_controller");
        $this->_book_model = $this->CI->load->model('Book_Model');
        $this->CI->add_session('testUserForSearch');
    }

    public function tearDown()
    {
        $this->CI->del_session();
    }

    public function testSearch()
    {
        $user_one_id = self::$user_id;
        $user_two_id = self::$user_id + 1;
        $books_for_user_one = array(
            array(
                "book_name" => "aaaa",
                "file_id"   => 0
            ),
            array(
                "book_name" => "aabb",
                "file_id"   => 0
            ),
            array(
                "book_name" => "cccc",
                "file_id"   => 0
            )
        );
        $books_for_user_two = array(
            array(
                "book_name" => "dddd",
                "file_id"   => 0
            )
        );
        foreach ($books_for_user_one as $book) {
            $this->CI->db->insert("book", $book);
            $insert_id = $this->CI->db->insert_id();
            $relation = array(
                "user_id" => $user_one_id,
                "book_id" => $insert_id
            );
            $this->CI->db->insert("user_book_relation", $relation);
        }
        foreach ($books_for_user_two as $book) {
            $this->CI->db->insert("book", $book);
            $insert_id = $this->CI->db->insert_id();
            $relation = array(
                "user_id" => $user_two_id,
                "book_id" => $insert_id
            );
            $this->CI->db->insert("user_book_relation", $relation);
        }
        $ret = $this->CI->do_search();
        $this->assertEquals(RESULT_MISSING_ARGS, $ret);
        // test book name
        $_GET['book_name'] = "";
        $ret = $this->CI->do_search();
        $this->assertEquals(RESULT_MISSING_ARGS, $ret);
        unset($_GET);
        $_GET['book_name'] = "aa";
        $ret = $this->CI->do_search();
        $this->assertEquals(RESULT_SUCCESS, $ret);
        unset($_GET);
        $_GET['book_name'] = "ee";
        $ret = $this->CI->do_search();
        $this->assertEquals(RESULT_NO_BOOK, $ret);
        unset($_GET);
        // test own
        $_GET['own'] = true;
        $ret = $this->CI->do_search();
        $this->assertEquals(RESULT_SUCCESS, $ret);
        unset($_GET);
        $_GET['own'] = "test";
        $ret = $this->CI->do_search();
        $this->assertEquals(RESULT_MISSING_ARGS, $ret);
        unset($_GET);
        $_GET['own'] = true;
        $_GET['book_name'] = "ee";
        $ret = $this->CI->do_search();
        $this->assertEquals(RESULT_NO_BOOK, $ret);
        unset($_GET);
        $_GET['own'] = true;
        $_GET['book_name'] = "aa";
        $ret = $this->CI->do_search();
        $this->assertEquals(RESULT_SUCCESS, $ret);
        unset($_GET);
        $_GET['own'] = true;
        $_GET['book_name'] = "dd";
        $ret = $this->CI->do_search();
        $this->assertEquals(RESULT_NO_BOOK, $ret);
        unset($_GET);
    }
}