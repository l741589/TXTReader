<?php
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-11-20
 * Time: 下午7:44
 */

class UserModelTest extends CIUnit_TestCase
{

    protected $data = array(
        'username' => 'test_user_1',
        'password' => 'test_password'
    );
    protected $new_data = array(
        'username' => 'test_user_1',
        'password' => 'new_password'
    );
    private $_model;

    public function __construct()
    {
        parent::__construct();
        $this->CI->load->model('user_model');
        $this->_model = $this->CI->user_model;
    }

    public static function setUpBeforeClass()
    {
        $conn = new mysqli("localhost:3306", "root", "123456", "txtreader");
        $conn->autocommit(false);
        $conn->query("delete from user");
        $conn->query("delete from user_book_relation");
        if (!$conn->errno) {
            $conn->commit();
            echo("Database is ready");
        } else {
            $conn->rollback();
            echo("Database is not ready");
        }
    }

    public function testAddUser()
    {
        // test username validation
        $ret = $this->_model->add_user("test", $this->data['password']);
        $this->assertEquals(RESULT_INVALID_USERNAME, $ret);
        $ret = $this->_model->add_user("te#@st", $this->data['password']);
        $username = "";
        for ($i = 0; $i < 34; $i++) {
            $username .= "t";
        }
        $ret = $this->_model->add_user($username, $this->data['password']);
        $this->assertEquals(RESULT_INVALID_USERNAME, $ret);
        // test right username
        $ret = $this->_model->add_user($this->data['username'], $this->data['password']);
        $this->assertEquals(RESULT_SUCCESS, $ret);
        // test same username
        $ret = $this->_model->add_user($this->data['username'], $this->data['password']);
        $this->assertEquals(RESULT_SAME_USERNAME, $ret);
    }

    public function testGetByUsername()
    {
        $row = $this->_model->get_by_username($this->data['username']);
        $this->assertNotEquals(false, $row);
        $row = $this->_model->get_by_username('test_user_2');
        $this->assertEquals(false, $row);
    }

    public function testPasswordCheck()
    {
        $res = $this->_model->password_check($this->data['username'],
            $this->data['password']);
        $this->assertEquals(RESULT_SUCCESS, $res);
        $res = $this->_model->password_check($this->data['username'], 'aaaa');
        $this->assertEquals(RESULT_PASSWD_ERROR, $res);
    }

    public function testUpdateUser()
    {
        $user = $this->_model->get_by_username($this->new_data['username']);
        $this->_model->update_user($this->new_data, $user->id);
        $ret = $this->_model->password_check($this->new_data['username'],
            $this->new_data['password']);
        $this->assertEquals(RESULT_SUCCESS, $ret);
    }

    public function testGetBookIds()
    {
        $user = $this->_model->get_by_username($this->new_data['username']);
        $ret = $this->_model->get_book_ids($this->new_data['username']);
        $this->assertEquals(RESULT_NO_BOOK, $ret);
        $book_id = 1;
        $this->CI->db->insert("user_book_relation", array(
            "user_id" => $user->id,
            "book_id" => $book_id
        ));
        $ret = $this->_model->get_book_ids($this->new_data['username']);
        $this->assertNotEquals(RESULT_NO_BOOK, $ret);
        $this->assertEquals($book_id, $ret[0]);
    }
} 