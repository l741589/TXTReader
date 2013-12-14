<?php if (!defined('BASEPATH')) exit('No direct script access allowed');
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-11-22
 * Time: 下午10:30
 */

class Book_Model extends CI_Model
{

    function __construct()
    {
        parent::__construct();
        $this->load->database();
    }

    function save_book($options = array())
    {
        $require_data = array("user_id", "file_md5", "book_name", "file_data");
        if (!$this->_required($require_data, $options)) {
            return RESULT_MISSING_ARGS;
        }
        $book_info = $this->_get_by_file_md5($options['file_md5']);
        if ($book_info != false) {
            $book_id = $book_info->id;
        } else {
            $this->db->set('book_name', $options['book_name']);
            $this->db->set('file_md5', $options['file_md5']);
            $this->db->insert('book');
            $book_id = $this->db->insert_id();
            if ($this->db->affected_rows() <= 0) {
                return RESULT_DB_ERROR;
            }
            $this->db->set('book_id', $book_id);
            $this->db->set('file_data', $options['file_data']);
            $this->db->insert('file');
            if ($this->db->affected_rows() <= 0) {
                return RESULT_DB_ERROR;
            }
        }
//        $book_id = $book_info->id;
        $user_id = $options['user_id'];
        if (!$this->_is_user_has_book($user_id, $book_id)) {
            $this->db->set('book_id', $book_id);
            $this->db->set('user_id', $user_id);
            $this->db->insert('user_book_relation');
            if ($this->db->affected_rows() <= 0) {
                return RESULT_DB_ERROR;
            }
        }
        return RESULT_SUCCESS;
    }

    function find_books($book_name)
    {
        $this->db->like("book_name", $book_name);
        $query = $this->db->get("book");
        if ($this->db->affected_rows() <= 0) {
            return false;
        }
        $result = array();
        foreach ($query->result() as $row) {
            $book_info = array(
                "book_id"   => $row->id,
                "book_name" => $row->book_name
            );
            $result[] = $book_info;
        }
        return $result;
    }

    function get_book($book_id)
    {
        echo $book_id;
        $this->db->where("id", $book_id);
        $query = $this->db->get("book");
        if ($this->db->affected_rows() <= 0) {
            return false;
        }
        $row = $query->first_row();
        $book_name = $row->book_name;
        return array(
            "book_id"   => $book_id,
            "book_name" => $book_name
        );
    }

    function get_file_data($book_id)
    {
        $this->db->where("book_id", $book_id);
        $query = $this->db->get("file");
        $row = $query->first_row();
        $file_data = $row->file_data;
        return $file_data;
    }

    function _required($required, $data)
    {
        foreach ($required as $field)
            if (!isset($data[$field])) {
                return false;
            }
        return true;
    }

    function _is_user_has_book($user_id, $book_id)
    {
        $this->db->where('user_id', $user_id);
        $this->db->where('book_id', $book_id);
        $query = $this->db->get('user_book_relation');
        $row = $query->first_row();
        if ($this->db->affected_rows() > 0) {
            return true;
        }
        return false;
    }

    function _is_existed_book($file_md5)
    {
        $this->db->where("file_md5", $file_md5);
        $query = $this->db->get("book");
        $row = $query->first_row();
        if ($this->db->affected_rows() > 0) {
            return true;
        }
        return false;
    }

    function _get_by_file_md5($file_md5)
    {
        $this->db->where("file_md5", $file_md5);
        $query = $this->db->get("book");
        $row = $query->first_row();
        if ($this->db->affected_rows() > 0) {
            return $row;
        }
        return false;
    }
}