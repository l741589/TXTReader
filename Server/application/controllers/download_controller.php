<?php if (!defined('BASEPATH')) exit('No direct script access allowed');
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-11-22
 * Time: 下午10:31
 */

require_once "session_controller.php";
require_once APPPATH . "/core/common.php";

class Download_Controller extends Session_Controller
{
    private $_book_model;

    function __construct()
    {
        parent::__construct();
        $this->load->model("book_model");
        $this->_book_model = $this->book_model;
    }

    function do_download()
    {
        $result_code = 0;
        $require = array("id");
        $form_data = array(
            "id" => $this->input->get("id")
        );
        if (!$this->is_logged_in()) {
            $result_code = RESULT_NOT_LOGIN;
        } else {
            if (!$this->_is_required_data($require, $form_data)) {
                $result_code = RESULT_MISSING_ARGS;
            } else {
                $book_id = $form_data['id'];
                $book_info = $this->_book_model->get_book_by_id($book_id);
                $book_file = $this->_book_model->get_filedata_by_id($book_info['file_id']);
                if (!$book_info || !$book_file) {
                    $result_code = RESULT_NO_BOOK;
                } else {
                    $this->load->helper('download');
                    $book_name = rawurldecode($book_info['book_name'] . ".txt");
                    force_download($book_name, $book_file);
                    $result_code = RESULT_SUCCESS;
                }
            }
        }
        show_result($result_code);
        return $result_code;
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