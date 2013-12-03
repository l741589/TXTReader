<?php
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-12-1
 * Time: 上午4:40
 */

require_once APPPATH."/core/session_controller.php";

class Search_Controller extends Session_Controller{

    private $_book_model;

    function __construct() {
        $this->load->model("book_model");
        $this->_book_model = $this->book_model;
    }

    function search($bookname) {
        $books = ($this->_book_model->find($bookname));
        if ($books) {
            return true;
        } else {
            return false;
        }
    }
} 