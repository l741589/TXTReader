<?php
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-11-20
 * Time: 下午1:52
 */

class User_Model extends CI_Model {
    var $username = "";
    var $password = "";

    function __construct() {
        parent::__construct();
        $this->load->database();
    }

    function add_user($username, $password) {
        $this->username = $username;
        $this->password = $password;
        if (!$this->is_valid_username())
            return false;
        $this->db->insert("users", $this);
        if ($this->db->affected_rows() > 0) {
            $this->login($username);
            return true;
        } else {
            return false;
        }
    }

    function update_user($username, $password) {
        $this->username = $username;
        $this->password = $password;
        $this->db->update('users', $this);
    }

    function is_valid_username() {
        return true;
    }

    function get_by_username($username) {
        $this->db->where('username', $username);
        $query = $this->db->get('users');
        if ($query->num_rows() == 1) {
            return $query->row();
        } else {
            return false;
        }
    }

    function password_check($username, $password) {
        if ($user = $this->get_by_username($username)) {
            return $user->password == $password ? true : false;
        } else {
            return false;
        }
    }

    function add_session($username) {
        $data = array("username" => $username, "logged_in" =>true);
        $this->session->set_userdata($data);
    }

    function del_session() {
        if ($this->logged_in == true) {
            $this->session->sess_destroy();
            return true;
        } else
            return false;
    }
}