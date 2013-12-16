<?php if (!defined('BASEPATH')) exit('No direct script access allowed');
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-11-20
 * Time: 下午1:52
 */

require_once "session_controller.php";
require_once APPPATH . "/core/Common.php";

class User_Controller extends Session_Controller
{

    private $_user_model;

    public function __construct()
    {
        parent::__construct();
        $this->load->model('user_model');
        $this->_user_model = $this->user_model;
        $this->load->library('form_validation');
    }

    function index()
    {
        $this->load->helper('form');
        $this->load->view('login');
    }

    function signup()
    {
        $required = array('username', 'password');
        $form_data = array(
            'username'              => $this->input->post('username'),
            'password'              => $this->input->post('password'),
            'password_comfirmation' => $this->input->post('password_comfirmation')
        );
        $result_code = 0;
        if (!$this->_is_required_data($required, $form_data)) {
            $result_code = RESULT_MISSING_ARGS;
        } elseif ($form_data['password'] != $form_data['password_comfirmation']) {
            $result_code = RESULT_PASSWORDS_DIFFERENT;
        } else {
            $result_code = $this->_user_model->add_user(
                $form_data['username'], $form_data['password']
            );
            if ($result_code == RESULT_SUCCESS) {
                $this->add_session($form_data['username']);
            }
        }
        show_result($result_code);
        return $result_code;
    }

    function login()
    {
        $required = array('username', 'password');
        $form_data = array(
            'username' => $this->input->post('username'),
            'password' => $this->input->post('password')
        );
        $_result_code = 0;
        if ($this->is_logged_in()) {
            $_result_code = RESULT_SUCCESS;
        } elseif (!$this->_is_required_data($required, $form_data)) {
            $_result_code = RESULT_MISSING_ARGS;
        } else {
            $_result_code = $this->_user_model->password_check(
                $form_data['username'], $form_data['password']);
            if ($_result_code == RESULT_SUCCESS) {
                $this->add_session($form_data['username']);
            }
        }
        show_result($_result_code);
        return $_result_code;;
    }

    function logout()
    {
        $_result_code = 0;
        if ($this->is_logged_in()) {
            $is_logged_out = $this->del_session();
            if ($is_logged_out) {
                $_result_code = RESULT_SUCCESS;
            } else {
                $_result_code = RESULT_CANNOT_LOGOUT;
            }
        } else {
            $_result_code = RESULT_NOT_LOGIN;
        }
        show_result($_result_code);
        return $_result_code;
    }

    function _is_required_data($required_fields, $form_data)
    {
        foreach ($required_fields as $field) {
            if (!isset($form_data[$field]) || empty($form_data[$field])) {
                echo $form_data[$field];
                return false;
            }
        }
        return true;
    }
}