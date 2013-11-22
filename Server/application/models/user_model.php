<?php if ( ! defined('BASEPATH')) exit('No direct script access allowed');
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-11-20
 * Time: 下午1:52
 */

class User_Model extends CI_Model {
    /**
     * @var string
     */
    var $username = "";
    /**
     * @var string
     */
    var $password = "";

    /**
     *
     */
    function __construct() {
        parent::__construct();
        $this->load->database();
    }

    /**
     * @param $username
     * @param $password
     * @return bool
     */
    function add_user($username, $password) {
        $this->username = $username;
        $this->password = $password;
        if (!$this->is_valid_username())
            return false;
        $this->db->insert("users", $this);
        if ($this->db->affected_rows() > 0) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * @param $username
     * @param $password
     */
    function update_user($username, $password) {
        $this->username = $username;
        $this->password = $password;
        $this->db->update('users', $this);
    }

    /**
     * @return bool
     */
    function _is_valid_username() {
        return true;
    }

    /**
     * @param $username
     * @return bool
     */
    function get_by_username($username) {
        $this->db->where('username', $username);
        $query = $this->db->get('users');
        if ($query->num_rows() == 1) {
            return $query->row();
        } else {
            return false;
        }
    }

    /**
     * @param $username
     * @param $password
     * @return bool
     */
    function password_check($username, $password) {
        if ($user = $this->get_by_username($username)) {
            return $user->password == $password ? true : false;
        } else {
            return false;
        }
    }
}