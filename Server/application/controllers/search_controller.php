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
        $book_name = $this->input->get('book_name');
        $is_owned = $this->input->get('own');
        if (isset($is_owned) && $is_owned == true) {
            if (!$this->is_logged_in()) {
                show_result(RESULT_NOT_LOGIN);
            } else {
                $this->load->model('user_model');
                $username = $this->get_current_username();
                $user_book_ids = (array)$this->user_model->get_book_ids($username, $book_name);
                if (count($user_book_ids) == 0) {
                    show_result(RESULT_NO_BOOK);
                } else {
                    $ret = array();
                    foreach ($user_book_ids as $id) {
                        $book = $this->_book_model->get_book($id);
                        if ($book) {
                            $ret[] = $book;
                        }
                    }
                    show_result(RESULT_SUCCESS, $ret);
                }
            }
        } else {
            if (isset($book_name) && !is_null($book_name)) {
                $books = $this->_book_model->find_books($book_name);
                if ($books) {
                    show_result(RESULT_SUCCESS, $books);
                } else {
                    show_result(RESULT_NO_BOOK);
                }
            }
        }
    }
} 