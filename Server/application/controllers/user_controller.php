<?php if ( ! defined('BASEPATH')) exit('No direct script access allowed');
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-11-20
 * Time: 下午1:52
 */

class User_Controller extends CI_Controller {

    private $_user_model;

    public function __construct() {
        parent::__construct();
        $this->load->model('user_model');
    }

    function signup() {
        $username = $this->input->post('username');
        $password = $this->input->post('password');
        if ($username && $password) {
            $this->user_model->new_user($username, $password);
        }
        else {
            echo "error 1";
        }
    }

    function login() {
        $this->load->library('form_validation');
        $this->load->helper('form');

        $this->form_validation->set_rules('username', "username", "require");
        $this->form_validation->set_rules("password", "password", "require");

        if ($this->form_validation->run() == false) {
            show_error("Require complete login data", 666);
            return false;
        } else {
            $username = $this->input->post('username');
            $password = $this->input->post('password');
            $this->CI->password_check();
        }
    }

    function logged_out() {

    }
}