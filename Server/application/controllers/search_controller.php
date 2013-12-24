<?php  if (!defined('BASEPATH')) exit('No direct script access allowed');
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-12-1
 * Time: 上午4:40
 */

require_once "session_controller.php";
require_once APPPATH . "/core/common.php";

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
        $result_code = 0;
        $ret = [];
        $book_name = $this->input->get('book_name');
        $is_owned = $this->input->get('own');
        if (isset($is_owned) && $is_owned == 1) {
            if (!$this->is_logged_in()) {
                $result_code = RESULT_NOT_LOGIN;
//                show_result(RESULT_NOT_LOGIN);
            } else {
                $this->load->model('user_model');
                $username = $this->get_current_username();
                $user_book_ids = $this->user_model->get_book_ids($username);
                if ($user_book_ids != false) {
                    foreach ($user_book_ids as $id) {
                        $book = $this->_book_model->get_book_by_id($id);
                        $pattern = "/" . $book_name . "/";
                        if ($book && preg_match($pattern, $book['book_name'])) {
                            $ret[] = array(
                                "book_id"   => $book["book_id"],
                                "book_name" => $book["book_name"]
                            );
                        }
                    }
                    if (sizeof($ret) == 0) {
                        $result_code = RESULT_NO_BOOK;
                    } else {
                        $result_code = RESULT_SUCCESS;
                    }
//                    show_result(RESULT_SUCCESS, $ret);
                }
            }
        } else {
            if (isset($book_name) && !empty($book_name)) {
                $ret = $this->_book_model->get_books_by_bookname($book_name);
                if ($ret) {
                    $result_code = RESULT_SUCCESS;
//                    show_result(RESULT_SUCCESS, $ret);
                } else {
                    $result_code = RESULT_NO_BOOK;
//                    show_result(RESULT_NO_BOOK);
                }
            } else {
                $result_code = RESULT_MISSING_ARGS;
            }
        }
        show_result($result_code, $ret);
        return $result_code;
    }
} 