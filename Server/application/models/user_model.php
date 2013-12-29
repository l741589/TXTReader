<?php if (!defined('BASEPATH')) exit('No direct script access allowed');
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-11-20
 * Time: 下午1:52
 */

class User_Model extends CI_Model
{
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
    function __construct()
    {
        parent::__construct();
        $this->load->database();
    }

    /**
     * @param $username
     * @param $password
     * @return bool
     */
    function add_user($username, $password)
    {
        $this->username = $username;
        $this->password = $password;
        if (!$this->_is_valid_username()) {
            return RESULT_INVALID_USERNAME;
        }
        if (!$this->_is_exist_username($username)) {
            return RESULT_SAME_USERNAME;
        }
        $this->db->insert("user", $this);
        if ($this->db->affected_rows() <= 0) {
            return RESULT_DB_ERROR;
        }
        return RESULT_SUCCESS;
    }

    /**
     * @param $username
     * @param $password
     */
    function update_user($username, $password)
    {
        $this->username = $username;
        $this->password = $password;
        $this->db->update('user', $this);
    }

    function password_check($username, $password)
    {
        if ($user = $this->_get_by_username($username)) {
            return $user->password == $password ?
                RESULT_SUCCESS : RESULT_PASSWD_ERROR;
        } else {
            return RESULT_USER_NOT_EXIST;
        }
    }

    function get_book_ids($username)
    {
        $user = $this->_get_by_username($username);
        $this->db->where("user_id", $user->id);
        $query = $this->db->get("user_book_relation");
        $ret = array();
        foreach ($query->result() as $row) {
            $ret[] = $row->book_id;
        }
        return $ret;
    }

    /**
     * @return bool
     */
    function _is_valid_username()
    {
        return true;
    }

    function _is_exist_username($username)
    {
        $this->db->where('username', $username);
        $this->db->get('user');
        if ($this->db->affected_rows() > 0) {
            return false;
        }
        return true;
    }

    /**
     * @param $username
     * @return bool
     */
    function _get_by_username($username)
    {
        $this->db->where('username', $username);
        $query = $this->db->get('user');
        if ($query->num_rows() == 1) {
            return $query->first_row();
        } else {
            return false;
        }
    }
}