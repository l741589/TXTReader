<?php if ( ! defined('BASEPATH')) exit('No direct script access allowed');
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-11-20
 * Time: 下午1:52
 */

class User_Controller extends Session_Controller {

    private $_user_model;

    public function __construct() {
        parent::__construct();
        $this->load->model('user_model');
        $this->_user_model = $this->user_model;
    }

    function signup() {
        $username = $this->input->post('username');
        $password = $this->input->post('password');
        if ($username != false && $password != false) {
            if ($this->user_model->add_user($username, $password)) {
                $this->add_session($username);
                return true;
            }
        }
        return false;
    }

    function login() {
        $this->load->library('form_validation');
        $this->load->helper('form');

        if ($this->is_logged_in()) {
            return true;
        } else {
            $this->form_validation->set_rules('username', "username", "require");
            $this->form_validation->set_rules("password", "password", "require");

            if ($this->form_validation->run() == false) {
                show_error("Require complete login data", 666);
                return false;
            } else {
                $username = $this->input->post('username');
                $password = $this->input->post('password');
                return $this->_user_model->password_check($username, $password);
            }
        }
    }

    function logged_out() {

    }
}