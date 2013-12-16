<?php if (!defined('BASEPATH')) exit('No direct script access allowed');
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-11-20
 * Time: 下午7:43
 */

class UserControllerTest extends CIUnit_TestCase
{
    protected $data_signup = array(
        'username' => 'test_user_2',
        'password'              => 'test_password',
        'password_comfirmation' => 'test_password'
    );

    protected $data_login = array(
        'username'              => 'test_user_2',
        'password'              => 'test_password',
        'password_comfirmation' => 'test_password'
    );

    public static function setUpBeforeClass()
    {
        $conn = new mysqli("localhost:3306", "root", "123456", "txtreader");
        $conn->autocommit(false);
        $conn->query("delete from user");
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
        $this->CI = set_controller("User_Controller");
    }

    public function testSignUp()
    {
        $_POST = array("username" => "test_user_2");
        $ret = $this->CI->signup();
        $this->assertEquals(RESULT_MISSING_ARGS, $ret);
        $_POST = array(
            "username"              => "test",
            "password"              => "test_password",
            "password_comfirmation" => "test_password"
        );
        $ret = $this->CI->signup();
        $this->assertEquals(RESULT_INVALID_USERNAME, $ret);
        $_POST = array(
            "username"              => "test_user_1",
            "password"              => "test_password",
            "password_comfirmation" => "test_password1"
        );
        $ret = $this->CI->signup();
        $this->assertEquals(RESULT_PASSWORDS_DIFFERENT, $ret);
        $_POST = $this->data_signup;
        $ret = $this->CI->signup();
        $this->assertEquals(RESULT_SUCCESS, $ret);
        // delete session
        $this->CI->logout();
    }

    public function testLoginAndLogout()
    {
        $_POST = array("username" => "test_user_2");
        $ret = $this->CI->login();
        $this->assertEquals(RESULT_MISSING_ARGS, $ret);
        $_POST = $this->data_login;
        $res = $this->CI->login();
        $this->assertEquals(RESULT_SUCCESS, $res);
        // test re-login
        $_POST = $this->data_login;
        $res = $this->CI->login();
        $this->assertEquals(RESULT_SUCCESS, $res);
        // test logout
        $ret = $this->CI->logout();
        $this->assertEquals(RESULT_SUCCESS, $ret);
        $ret = $this->CI->logout();
        $this->assertEquals(RESULT_NOT_LOGIN, $ret);
    }
}