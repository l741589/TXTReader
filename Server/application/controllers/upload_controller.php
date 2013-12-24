<?php if (!defined('BASEPATH')) exit('No direct script access allowed');
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-11-22
 * Time: 下午8:30
 */

require_once "session_controller.php";
require_once APPPATH . "/core/common.php";

class Upload_Controller extends Session_Controller
{

    private $_book_model;

    function __construct()
    {
        parent::__construct();
        $this->load->model("book_model");
        $this->_book_model = $this->book_model;
//        $this->add_session("test_user_1");
    }

    function index()
    {
        $this->load->helper('form');
        $this->load->view("upload_form", array('error' => ' '));
    }

    function do_upload()
    {
        $require = array("file_md5", "book_name");
        $form_data = array(
            "file_md5"  => $this->input->post("file_md5"),
            "book_name" => $this->input->post("book_name")
        );
        if (!$this->is_logged_in()) {
            show_result(RESULT_NOT_LOGIN);
            return RESULT_NOT_LOGIN;
        } else {
            if ($this->_is_required_data($require, $form_data)) {
                if (!$this->_book_model->is_file_existed($form_data['file_md5'])) {
                    show_result(RESULT_NO_FILE);
                    return RESULT_NO_FILE;
                } else {
                    $file_data = array(
                        "file_md5"  => $form_data["file_md5"],
                        "book_name" => $form_data["book_name"]
                    );
                    $result_code = $this->_save_book($file_data, true);
                    $book_id = $this->_book_model->inserted_book_id();
                    if ($result_code != RESULT_SUCCESS) {
                        show_result($result_code);
                    } else {
                        show_result($result_code, array("book_id" => $book_id));
                    }
                    return $result_code;
                }
            } else {
                $this->_upload_config();
                if (!$this->upload->do_upload()) {
                    $this->upload->error_msg;
                    $result_code = $this->upload->error_code;
                    show_result($result_code);
                    return $result_code;
                } else {
                    $file_data = $this->upload->data();
                    $result_code = $this->_save_book($file_data, false);
                    $book_id = $this->_book_model->inserted_book_id();
                    if ($result_code != RESULT_SUCCESS) {
                        show_result($result_code);
                    } else {
                        show_result($result_code, array("book_id" => $book_id));
                    }
                    return $result_code;
                }
            }
        }
    }

    function _upload_config()
    {
        $this->load->helper("string");
        $this->load->helper('url');
        $config['file_name'] = random_string("alnum", 32);
        $config['upload_path'] = getcwd() . './tmp/';
        $config['allowed_types'] = 'txt';
        $config['max_size'] = '15360';
        $this->load->library("upload", $config);
    }

    function _save_book($book_data, $is_exist)
    {
        $this->load->model("user_model");
        $user = $this->user_model->get_by_username($this->get_current_username());
        $file_info = null;
        if ($is_exist) {
            $file_info = array(
                'user_id'   => $user->id,
                'file_md5'  => $book_data["file_md5"],
                'book_name' => $book_data['book_name'],
            );
        } else {
            $file_info = array(
                'user_id'   => $user->id,
                'file_md5'  => md5_file($book_data['full_path']),
                'book_name' => $book_data['client_name'],
                'file_data' => file_get_contents($book_data['full_path'])
            );
//            $this->_delete_upload_file($book_data['full_path']);
        }
        $ret = $this->_book_model->save_book($file_info);
        return $ret;
    }

    function _delete_upload_file($file_path)
    {
        $this->load->helper('file');
        if (file_exists($file_path) && is_readable($file_path)) {
            unlink($file_path);
        }
    }

    function _is_required_data($required_fields, $form_data)
    {
        foreach ($required_fields as $field) {
            if (!isset($form_data[$field]) || empty($form_data[$field])) {
                return false;
            }
        }
        return true;
    }
}