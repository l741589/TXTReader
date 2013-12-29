<?php
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-12-5
 * Time: 上午2:27
 */

class BookModelTest extends CIUnit_TestCase
{
    protected $_file_info;
    private $_book_model;
    private static $user_id;

    public function __construct()
    {
        parent::__construct();
    }

    public static function setUpBeforeClass()
    {
        $_test_user = array(
            'username' => 'testUserForBookModel',
            'password' => 'testPassword'
        );
        $conn = new mysqli("localhost:3306", "root", "123456", "txtreader");
        $conn->autocommit(false);
        $conn->query("delete from user");
        $conn->query("delete from book");
        $conn->query("delete from file");
        $conn->query("delete from user_book_relation");
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
        parent::setUp();
        $this->CI->load->model('Book_Model');
        $this->_book_model = $this->CI->Book_Model;
        $dir = dirname(__DIR__);
        $realFileDir = $dir . "/testfiles/test1.txt";
        $this->_file_info = array(
            "user_id"  => self::$user_id,
            "book_name" => "file.txt",
            "file_md5" => md5_file($realFileDir),
            "file_data" => mysql_real_escape_string(file_get_contents($realFileDir))
        );
    }

    public function testSaveBook()
    {
        $ret = $this->_book_model->save_book($this->_file_info);
        $this->assertEquals(RESULT_SUCCESS, $ret);
    }

    public function testGetBooksByName()
    {
        $ret = $this->_book_model->get_books_by_bookname("file");
        $this->assertNotEquals(false, $ret);
        $this->assertEquals(1, sizeof($ret));
        $ret = $this->_book_model->get_books_by_bookname("wrong_book_name");
        $this->assertEquals(false, $ret);
    }

    public function testGetBookById()
    {
        $book_id = $this->_book_model->inserted_book_id();
        $book = $this->_book_model->get_book_by_id($book_id);
        $this->assertEquals("file", $book['book_name']);
    }
}