<?php if ( ! defined('BASEPATH')) exit('No direct script access allowed');
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-11-22
 * Time: 下午8:30
 */

require_once APPPATH."/core/session_controller.php";

class Upload_Controller extends Session_Controller {

    private $_book_model;

    function __construct() {
        parent::__construct();
        $this->load->model("book_model");
        $this->_book_model = $this->book_model;
//        $this->add_session("test_user_1");
    }

    function index() {
        $this->load->helper('form');
        $this->load->view("upload_form", array('error' => ' ' ));
    }

    function do_upload() {
        if ( ! $this->_session_check()) {
            echo "not login";
            return false;
        }
//        $file_md5 = $this->input->post("filemd5");
//        if ($this->_book_model->is_existed($file_md5)) {
//            return true;
//        }
        $this->_upload_config();
        if ( ! $this->upload->do_upload()) {
            $error = array(
                'error' => $this->upload->display_errors()
            );
            var_dump($error);
            return false;
        } else {
            $data = $this->upload->data();
            $ret = $this->_save_book($data);
            return $ret;
        }
    }

    function _upload_config() {
        $this->load->helper("string");
        $this->load->helper('url');
        $config['file_name'] = random_string("alnum", 32);
        $config['upload_path'] = getcwd() . './tmp/';
        $config['allowed_types'] = 'txt';
        $config['max_size'] = '1024';
        $this->load->library("upload", $config);
    }

    function _session_check() {
            $res = $this->is_logged_in();
        return $res;
    }

    function _save_book($book_data) {
        $this->load->model("user_model");
        $user = $this->user_model->get_by_username($this->get_current_username());
        $file_info = array(
            'user_id' => $user->id,
            'file_md5' => md5_file($book_data['full_path']),
            'book_name' => $book_data['client_name'],
            'file_data' => file_get_contents($book_data['full_path'])
        );
        $ret = $this->_book_model->save_book($file_info);
        var_dump($file_info);
        $this->_delete_upload_file($book_data['full_path']);
        return $ret;
    }

    function _delete_upload_file($file_path) {
        $this->load->helper('file');
        if (file_exists($file_path) && is_readable($file_path)) {
            unlink($file_path);
        }
    }
} 