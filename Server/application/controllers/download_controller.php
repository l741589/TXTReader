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
        parent::__construct();
        $this->load->model("book_model");
        $this->_book_model = $this->book_model;
    }

    function do_download() {
        $book_id = $this->input->get("id");
        if (!$book_id) {
            echo("No input");
            return false;
        }
        $book_info = $this->_book_model->get_book($book_id);
        if (!$book_info) {
            echo("No such book");
            return false;
        } else {
            $this->load->helper('download');
            force_download($book_info['book_name'], $book_info['file_data']);
            echo "dowload" . $book_id;
            return true;
        }
    }
} 