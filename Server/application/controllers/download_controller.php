<?php
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-11-22
 * Time: 下午10:31
 */

require_once APPPATH."/core/session_controller.php";
class Download_Controller extends Session_Controller{

    private $_book_model;

    function __construct() {
        $this->load->model("book_model");
        $this->_book_model = $this->book_model;
    }

    function download() {
        $book_id = $this->input->post("book_id");
        $book_info = $this->_book_model->get_book($book_id);
        if (!$book_info) {
            return false;
        } else {
            force_download($book_info['book_name'], $book_info['file_data']);
        }
    }
} 