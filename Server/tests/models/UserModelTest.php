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

    public function setUp()
    {
        parent::setUp();
        $this->CI->load->model('user_model');
        $this->_model = $this->CI->user_model;
    }

    public function testNewUser()
    {
        // clear db before test
        $this->clearDb();
        $this->_model->add_user($this->data['username'], $this->data['password']);
        $this->CI->db->where('username', $this->data['username']);
        $query = $this->CI->db->get('user');
        $this->assertEquals(1, $query->num_rows());
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
        $this->assertEquals(true, $res);
        $res = $this->_model->password_check($this->data['username'], 'aaaa');
        $this->assertEquals(false, $res);
    }

    public function testUpdateUser()
    {
        $this->_model->update_user($this->new_data['username'], $this->new_data['password']);
        $this->CI->db->where('username', $this->new_data['username']);
        $query = $this->CI->db->get('user');
        $row = $query->first_row('array');
        $this->assertEquals($this->new_data['password'], $row['password']);
    }

    function testValidUsername()
    {

    }

    private function clearDb()
    {
        $this->CI->db->where('username', $this->data['username']);
        $this->CI->db->delete('user');
    }
} 