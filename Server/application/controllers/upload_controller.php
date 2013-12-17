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
        if (!$this->is_logged_in()) {
            show_result(RESULT_NOT_LOGIN);
        } else {
            $this->_upload_config();
            if (!$this->upload->do_upload()) {
                $this->upload->error_msg;
                $error = array(
                    'error' => $this->upload->display_errors()
                );
                show_result(RESULT_UPLOAD_ERROR, $error);
            } else {
                $data = $this->upload->data();
                $result_code = $this->_save_book($data);
                $book_id = $this->_book_model->inserted_book_id();
                show_result($result_code, array("book_id" => $book_id));
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
        $config['max_size'] = '1024';
        $this->load->library("upload", $config);
    }

    function _save_book($book_data)
    {
        $this->load->model("user_model");
        $user = $this->user_model->get_by_username($this->get_current_username());
        $file_info = array(
            'user_id'   => $user->id,
            'file_md5'  => md5_file($book_data['full_path']),
            'book_name' => $book_data['client_name'],
            'file_data' => file_get_contents($book_data['full_path'])
        );
        $ret = $this->_book_model->save_book($file_info);
        $this->_delete_upload_file($book_data['full_path']);
        return $ret;
    }

    function _delete_upload_file($file_path)
    {
        $this->load->helper('file');
        if (file_exists($file_path) && is_readable($file_path)) {
            unlink($file_path);
        }
    }
} 