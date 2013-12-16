<?php  if (!defined('BASEPATH')) exit('No direct script access allowed');
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-12-1
 * Time: 上午4:40
 */

require_once "session_controller.php";
require_once APPPATH . "/core/Common.php";

class Search_Controller extends Session_Controller
{

    private $_book_model;

    function __construct()
    {
        parent::__construct();
        $this->load->model("book_model");
        $this->_book_model = $this->book_model;
    }

    function do_search()
    {
        $_result_code = 0;
        $book_name = $this->input->get('book_name');
        $is_owned = $this->input->get('own');
        if (isset($is_owned) && $is_owned == 1) {
            if (!$this->is_logged_in()) {
                show_result(RESULT_NOT_LOGIN);
            } else {
                $this->load->model('user_model');
                $username = $this->get_current_username();
                $user_book_ids = $this->user_model->get_book_ids($username);
                if ($user_book_ids === false) {
                    show_result(RESULT_NO_BOOK);
                } else {
                    $ret = array();
                    foreach ($user_book_ids as $id) {
                        $book = $this->_book_model->get_book_by_id($id);
                        $pattern = "/" . $book_name . "/";
                        if ($book && preg_match($pattern, $book['book_name'])) {
                            $ret[] = $book;
                        }
                    }
                    show_result(RESULT_SUCCESS, $ret);
                }
            }
        } else {
            if (isset($book_name) && !is_null($book_name)) {
                $books = $this->_book_model->get_books_by_bookname($book_name);
                if ($books) {
                    show_result(RESULT_SUCCESS, $books);
                } else {
                    show_result(RESULT_NO_BOOK);
                }
            }
        }
    }
} 