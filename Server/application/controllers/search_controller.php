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
        parent::__construct();
        $this->load->model("book_model");
        $this->_book_model = $this->book_model;
    }

    function do_search() {
        $book_name = $this->input->get('book_name');
//        $own = $this->input->get('own');
        if (!$book_name) {
            return false;
        }
        $books = $this->_book_model->find_book($book_name);
        if ($books) {
            print_r($books);
            return true;
        } else {
            echo("no book");
            return false;
        }
    }
} 