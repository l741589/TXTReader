<?php if (!defined('BASEPATH')) exit('No direct script access allowed');
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-11-22
 * Time: 下午10:31
 */

require_once "session_controller.php";
require_once APPPATH . "/core/Common.php";
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
        $book_id = $this->input->get("id");
        if (!$book_id) {
            show_result(RESULT_MISSING_ARGS);
        }
        $book_info = $this->_book_model->get_book($book_id);
        $book_file = $this->_book_model->get_file_data($book_id);
        if (!$book_info) {
            show_result(RESULT_NO_BOOK);
        } else {
            $this->load->helper('download');
            force_download($book_info['book_name'], $book_file);
//            show_result(RESULT_SUCCESS, $book_info);
        }
    }
} 