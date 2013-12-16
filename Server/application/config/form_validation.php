<?php if (!defined('BASEPATH')) exit('No direct script access allowed');
/**
 * Created by PhpStorm.
 * User: Limbo
 * Date: 13-12-17
 * Time: 上午3:22
 */

$config = array(
    'signup' => array(
        array(
            'field' => 'username',
//            'label' => 'Username',
            'rules' => 'required'
        ),
        array(
            'field' => 'password',
//            'label' => 'Password',
            'rules' => 'required'
        )
    )
);