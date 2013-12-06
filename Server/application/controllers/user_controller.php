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
    }

    function index()
    {
        $this->load->helper('form');
        $this->load->view('login');
    }

    function signup()
    {
        $username = $this->input->post('username');
        $password = $this->input->post('password');
        if ($username != false && $password != false) {
            $result_code = $this->_user_model->add_user($username, $password);
            if ($result_code == RESULT_SUCCESS) {
                $this->add_session($username);
                show_result($result_code);
            }
            show_result($result_code);
        } else {
            show_result(RESULT_MISSING_ARGS);
        }
    }

    function login()
    {
        $this->load->library('form_validation');
        $this->load->helper('form');
        $this->load->helper('url');

        $_result_code = 0;

        if ($this->is_logged_in()) {
            $_result_code = RESULT_SUCCESS;
        } else {
            $this->form_validation->set_rules('username', "username", "require");
            $this->form_validation->set_rules("password", "password", "require");

            if ($this->form_validation->run() == false) {
                $_result_code = RESULT_MISSING_ARGS;
            } else {
                $username = $this->input->post('username');
                $password = $this->input->post('password');
                $_result_code = $this->_user_model
                    ->password_check($username, $password);
                if ($_result_code == RESULT_SUCCESS) {
                    $this->add_session($username);
                }
            }
        }
        show_result($_result_code);
        return $_result_code == RESULT_SUCCESS;
    }

    function logout()
    {
        return $this->del_session();
    }
}