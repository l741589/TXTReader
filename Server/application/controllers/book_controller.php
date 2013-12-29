<?php
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-12-28
 * Time: 下午10:04
 */

require_once "session_controller.php";
require_once APPPATH . "/core/common.php";

class Book_Controller extends Session_Controller {
    private $_book_model;

    function __construct() {
        parent::__construct();
        $this->load->model("book_model");
        $this->_book_model = $this->book_model;
    }

    function index() {
//        $this->load->helper('form');
//        $this->load->view("upload_form", array('error' => ' '));
        $this->load->view('index');
    }

    function do_search() {
        $result_code = 0;
        $ret = [];
        $book_name = $this->input->get('book_name');
        $is_owned = $this->input->get('own');
        if (isset($is_owned) && $is_owned == 1) {
            if (!$this->is_logged_in()) {
                $result_code = RESULT_NOT_LOGIN;
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
                                "book_id" => $book["book_id"],
                                "book_name" => $book["book_name"]
                            );
                        }
                    }
                    if (sizeof($ret) == 0) {
                        $result_code = RESULT_NO_BOOK;
                    } else {
                        $result_code = RESULT_SUCCESS;
                    }
                }
            }
        } else {
            if (isset($book_name) && !empty($book_name)) {
                $ret = $this->_book_model->get_books_by_bookname($book_name);
                if ($ret) {
                    $result_code = RESULT_SUCCESS;
                } else {
                    $result_code = RESULT_NO_BOOK;
                }
            } else {
                $result_code = RESULT_MISSING_ARGS;
            }
        }
        show_result($result_code, $ret);
        return $result_code;
    }

    function do_download() {
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

    function do_upload() {
        $require = array("file_md5", "book_name");
        $form_data = array(
            "file_md5" => $this->input->post("file_md5"),
            "book_name" => $this->input->post("book_name")
        );
        if (!$this->is_logged_in()) {
            show_result(RESULT_NOT_LOGIN);
            return RESULT_NOT_LOGIN;
        } else {
            if ($this->_is_required_data($require, $form_data)) {
                if (!$this->_book_model->is_file_existed($form_data['file_md5'])) {
                    show_result(RESULT_NO_FILE);
                    return RESULT_NO_FILE;
                } else {
                    $file_data = array(
                        "file_md5" => $form_data["file_md5"],
                        "book_name" => $form_data["book_name"]
                    );
                    $result_code = $this->_save_book($file_data, true);
                    $book_id = $this->_book_model->inserted_book_id();
                    if ($result_code != RESULT_SUCCESS) {
                        show_result($result_code);
                    } else {
                        show_result($result_code, array("book_id" => $book_id));
                    }
                    return $result_code;
                }
            } else {
                $this->_upload_config();
                if (!$this->upload->do_upload()) {
                    $this->upload->error_msg;
                    $result_code = $this->upload->error_code;
                    show_result($result_code);
                    return $result_code;
                } else {
                    $file_data = $this->upload->data();
                    $result_code = $this->_save_book($file_data, false);
                    $book_id = $this->_book_model->inserted_book_id();
                    if ($result_code != RESULT_SUCCESS) {
                        show_result($result_code);
                    } else {
                        show_result($result_code, array("book_id" => $book_id));
                    }
                    return $result_code;
                }
            }
        }
    }

    function _upload_config() {
        $this->load->helper("string");
        $this->load->helper('url');
        $config['file_name'] = random_string("alnum", 32);
        $config['upload_path'] = getcwd() . './tmp/';
        $config['allowed_types'] = 'txt';
        $config['max_size'] = '15360';
        $this->load->library("upload", $config);
    }

    function _save_book($book_data, $is_exist) {
        $this->load->model("user_model");
        $user = $this->user_model->get_by_username($this->get_current_username());
        $file_info = null;
        if ($is_exist) {
            $file_info = array(
                'user_id' => $user->id,
                'file_md5' => $book_data["file_md5"],
                'book_name' => $book_data['book_name'],
            );
        } else {
            $file_info = array(
                'user_id' => $user->id,
                'file_md5' => md5_file($book_data['full_path']),
                'book_name' => $book_data['client_name'],
                'file_data' => file_get_contents($book_data['full_path'])
            );
//            $this->_delete_upload_file($book_data['full_path']);
        }
        $ret = $this->_book_model->save_book($file_info);
        return $ret;
    }

    function _delete_upload_file($file_path) {
        $this->load->helper('file');
        if (file_exists($file_path) && is_readable($file_path)) {
            unlink($file_path);
        }
    }

    function _is_required_data($required_fields, $form_data) {
        foreach ($required_fields as $field) {
            if (!isset($form_data[$field]) || empty($form_data[$field])) {
                return false;
            }
        }
        return true;
    }
} 