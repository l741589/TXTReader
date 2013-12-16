<?php if (!defined('BASEPATH')) exit('No direct script access allowed');
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-11-20
 * Time: 下午1:52
 */

class User_Model extends CI_Model
{
    function __construct()
    {
        parent::__construct();
        $this->load->database();
    }

    function add_user($username, $password)
    {
        $data = array(
            "username" => $username,
            "password" => sha1($password)
        );
        if (!$this->_is_valid_username($username)) {
            return RESULT_INVALID_USERNAME;
        }
        if (!$this->_is_exist_username($username)) {
            return RESULT_SAME_USERNAME;
        }
        $this->db->insert("user", $data);
        if ($this->db->affected_rows() <= 0) {
            return RESULT_DB_ERROR;
        }
        return RESULT_SUCCESS;
    }

    function update_user($updated_datas, $id)
    {
        if (isset($updated_datas['password'])) {
            $updated_datas['password'] = sha1($updated_datas['password']);
        }
        $this->db->where("id", $id);
        $this->db->update('user', $updated_datas);
        if ($this->db->affected_rows() > 0) {
            return RESULT_SUCCESS;
        } else {
            return RESULT_DB_ERROR;
        }
    }

    function password_check($username, $password)
    {
        if ($user = $this->get_by_username($username)) {
            return $user->password == sha1($password) ?
                RESULT_SUCCESS : RESULT_PASSWD_ERROR;
        } else {
            return RESULT_USER_NOT_EXIST;
        }
    }

    function get_by_username($username)
    {
        $this->db->where('username', $username);
        $query = $this->db->get('user');
        if ($query->num_rows() == 1) {
            return $query->first_row();
        } else {
            return false;
        }
    }

    function get_book_ids($username)
    {
        $user = $this->get_by_username($username);
        $this->db->where("user_id", $user->id);
        $query = $this->db->get("user_book_relation");
        if ($query->num_rows() == 0) {
            return RESULT_NO_BOOK;
        }
        $ret = array();
        foreach ($query->result() as $row) {
            $ret[] = $row->book_id;
        }
        return $ret;
    }

    function _is_valid_username($username)
    {
        $pattern = '/^\w{6,32}$/';
        return preg_match($pattern, $username);
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
}