<?php if (!defined('BASEPATH')) exit('No direct script access allowed');
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-11-22
 * Time: 下午10:30
 */

class Book_Model extends CI_Model
{
    var $inserted_book_id;

    function __construct()
    {
        parent::__construct();
        $this->load->database();
    }

    function save_book($data = array())
    {
        // save file and get file id
        $file_id = 0;
        if ($this->_is_file_existed($data['file_md5'])) {
            $file = $this->_get_file_by_md5($data['file_md5']);
            $file_id = $file->id;
        } else {
            $file_data = array(
                'file_md5'  => $data['file_md5'],
                'file_data' => $data['file_data']
            );
            $this->db->insert('file', $file_data);
            if ($this->db->affected_rows() <= 0) {
                return RESULT_DB_ERROR;
            }
            $file_id = $this->db->insert_id();
        }

        // add book
        $book_data = array(
            'book_name' => $this->_remove_suffix($data['book_name'], "txt"),
            'file_id'   => $file_id
        );
        $this->db->insert('book', $book_data);
        if ($this->db->affected_rows() <= 0) {
            return RESULT_DB_ERROR;
        }
        $this->inserted_book_id = $book_id = $this->db->insert_id();

        // add user and book relation
        $relation_data = array(
            'user_id' => $data['user_id'],
            'book_id' => $book_id
        );
        $this->db->insert('user_book_relation', $relation_data);
        if ($this->db->affected_rows() <= 0) {
            return RESULT_DB_ERROR;
        }
        return RESULT_SUCCESS;
    }

    function get_books_by_bookname($book_name)
    {
        $this->db->like("book_name", $book_name);
        $query = $this->db->get("book");
        if ($query->num_rows() == 0) {
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

    function inserted_book_id()
    {
        return $this->inserted_book_id;
    }

    function get_book_by_id($book_id)
    {
        $this->db->where("id", $book_id);
        $query = $this->db->get("book");
        if ($this->db->affected_rows() <= 0) {
            return false;
        }
        $row = $query->first_row();
        return array(
            "book_id"   => $book_id,
            "file_id"   => $row->file_id,
            "book_name" => $row->book_name
        );
    }

    function get_filedata_by_id($file_id)
    {
        $this->db->where("id", $file_id);
        $query = $this->db->get("file");
        if ($query->num_rows() <= 0) {
            return false;
        }
        $row = $query->first_row();
        $file_data = $row->file_data;
        return $file_data;
    }

    function _is_user_has_book($user_id, $book_id)
    {
        $this->db->where('user_id', $user_id);
        $this->db->where('book_id', $book_id);
        $query = $this->db->get('user_book_relation');
        if ($query->num_rows() > 0) {
            return true;
        }
        return false;
    }

    function _is_file_existed($file_md5)
    {
        $this->db->where("file_md5", $file_md5);
        $this->db->get("file");
        if ($this->db->affected_rows() > 0) {
            return true;
        }
        return false;
    }

    function _get_file_by_md5($file_md5)
    {
        $this->db->where("file_md5", $file_md5);
        $query = $this->db->get("file");
        $row = $query->first_row();
        if ($this->db->affected_rows() > 0) {
            return $row;
        }
        return false;
    }

    function _remove_suffix($string, $suffix)
    {
        if (preg_match('/.+\.' . $suffix . '/', $string)) {
            $string = substr($string, 0, strlen($string) - strlen($suffix) - 1);
        }
        return $string;
    }
}