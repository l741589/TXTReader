<?php if ( ! defined('BASEPATH')) exit('No direct script access allowed');
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-11-20
 * Time: 下午1:52
 */

require_once APPPATH."/core/session_controller.php";

class User_Controller extends Session_Controller {

    private $_user_model;

    public function __construct() {
        parent::__construct();
        $this->load->model('user_model');
        $this->_user_model = $this->user_model;
    }

    function index() {
        $this->load->helper('form');
        $this->load->view('login');
    }

    function signup() {
        $username = $this->input->post('username');
        $password = $this->input->post('password');
        if ($username != false && $password != false) {
            if ($this->_user_model->add_user($username, $password)) {
                $this->add_session($username);
                echo "welcome";
                return true;
            }
        }
        echo "cannot signup";
        return false;
    }

    function login() {
        $this->load->library('form_validation');
        $this->load->helper('form');
        $this->load->helper('url');

        if ($this->is_logged_in()) {
            redirect("/Upload_Controller");
            return true;
        } else {
            $this->form_validation->set_rules('username', "username", "require");
            $this->form_validation->set_rules("password", "password", "require");

            if ($this->form_validation->run() == false) {
//                show_error("Require complete login data", 500);
                return false;
            } else {
                $username = $this->input->post('username');
                $password = $this->input->post('password');
                if ($this->_user_model->password_check($username, $password)) {
                    $this->add_session($username);
                    echo "login success";
                    redirect("/Upload_Controller");
                    return true;
                } else
                    return false;
            }
        }
    }

    function logged_out() {
        return $this->del_session();
    }
}