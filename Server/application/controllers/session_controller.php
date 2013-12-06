<?php if (!defined('BASEPATH')) exit('No direct script access allowed');
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-11-22
 * Time: 下午11:07
 */

class Session_Controller extends CI_Controller
{

    public function __construct()
    {
        parent::__construct();
        $this->load->library('session');
    }

    function add_session($username)
    {
        $data = array(
            "username"  => $username,
            "logged_in" => true
        );
        $this->session->set_userdata($data);
    }

    function del_session()
    {
        if ($this->is_logged_in()) {
            $this->session->sess_destroy();
            return true;
        } else {
            return false;
        }
    }

    function get_current_username()
    {
        return $this->session->userdata['username'];
    }

    function is_logged_in()
    {
        $is_logged_in = $this->session->userdata('logged_in');
        if (isset($is_logged_in) && $is_logged_in == true) {
            return true;
        } else {
            return false;
        }
    }
} 