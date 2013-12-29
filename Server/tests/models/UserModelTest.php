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
        'username' => '',
        'password' => ''
    );
    protected $username = "test_user_1";
    protected $password = "test_password";
    protected $new_password = "new_password";
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
        $conn->query("DELETE FROM user");
        $conn->query("DELETE FROM user_book_relation");
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
        $this->data['username'] = "test";
        $this->data['password'] = $this->password;
        $ret = $this->_model->add_user($this->data);
        $this->assertEquals(RESULT_INVALID_USERNAME, $ret);
        $this->data['username'] = "te#@st";
        $ret = $this->_model->add_user($this->data);
        $username = "";
        for ($i = 0; $i < 34; $i++) {
            $username .= "t";
        }
        $this->data['username'] = $username;
        $ret = $this->_model->add_user($this->data);
        $this->assertEquals(RESULT_INVALID_USERNAME, $ret);
        // test right username
        $this->data['username'] = $this->username;
        $ret = $this->_model->add_user($this->data);
        $this->assertEquals(RESULT_SUCCESS, $ret);
        // test same username
        $ret = $this->_model->add_user($this->data);
        $this->assertEquals(RESULT_SAME_USERNAME, $ret);
    }

    public function testGetByUsername()
    {
        $row = $this->_model->get_by_username($this->username);
        $this->assertNotEquals(false, $row);
        $row = $this->_model->get_by_username('test_user_2');
        $this->assertEquals(false, $row);
    }

    public function testPasswordCheck()
    {
        $res = $this->_model->password_check(
            $this->username, $this->password);
        $this->assertEquals(RESULT_SUCCESS, $res);
        $res = $this->_model->password_check(
            $this->username, 'wrong_password');
        $this->assertEquals(RESULT_PASSWD_ERROR, $res);
        $res = $this->_model->password_check(
            'test_user_2', 'wrong_password');
        $this->assertEquals(RESULT_USER_NOT_EXIST, $res);
    }

    public function testUpdateUser()
    {
        $new_data = array(
            'password' => $this->new_password
        );
        $user = $this->_model->get_by_username($this->username);
        $this->_model->update_user($new_data, $user->id);
        $ret = $this->_model->password_check(
            $this->username, $this->new_password
        );
        $this->assertEquals(RESULT_SUCCESS, $ret);
    }

    public function testGetBookIds()
    {
        $user = $this->_model->get_by_username($this->username);
        $ret = $this->_model->get_book_ids($this->username);
        $this->assertEquals(false, $ret);
        $book_id = 1;
        $this->CI->db->insert("user_book_relation", array(
            "user_id" => $user->id,
            "book_id" => $book_id
        ));
        $ret = $this->_model->get_book_ids($this->username);
        $this->assertNotEquals(RESULT_NO_BOOK, $ret);
        $this->assertEquals($book_id, $ret[0]);
    }
} 