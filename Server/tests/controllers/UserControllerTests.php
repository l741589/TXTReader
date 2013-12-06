<?php if (!defined('BASEPATH')) exit('No direct script access allowed');
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-11-20
 * Time: 下午7:43
 */

class UserControllerTest extends CIUnit_TestCase
{

    protected $data = array(
        'username' => 'test_user_2',
        'password' => 'test_password'
    );

    public function setUp()
    {
        $this->CI = set_controller("User_Controller");
        $this->CI->load->library('session');
    }

    public function testSignUp()
    {
        // clear db before all test
        $this->clearDb();
        $_POST = $this->data;
        $this->CI->signup();
        $this->CI->db->where('username', $this->data['username']);
        $query = $this->CI->db->get('user');
        $this->assertEquals(1, $query->num_rows());
//        var_dump($this->CI->session->all_userdata());
//        // resignup
//        $res = $this->CI->signup();
//        $this->assertEquals(false, $res);
        $this->CI->del_session();
    }

    public function testLogin()
    {
        $_POST['username'] = $this->data['username'];
        $res = $this->CI->login();
        $this->assertEquals(false, $res);
        $_POST['password'] = $this->data['password'];
        $res = $this->CI->login();
        $this->assertEquals(true, $res);
        $sess_data = $this->CI->session->all_userdata();
        $this->assertEquals(true, $this->CI->is_logged_in());
//        $this->assertEquals(true, isset($sess_data['session_id']));
    }

    function clearDb()
    {
        $this->CI->db->where('username', $this->data['username']);
        $this->CI->db->delete('user');
    }
}