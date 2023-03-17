﻿$(document).ready(function () {

    $(document).on('click', '.deleteBtn', function (e) {
        e.preventDefault();

        let url = $(this).attr('href');
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) {
                fetch(url)
                    .then(res => res.text())
                    .then(data => {
                        $('.indexContainer').html(data)
                    })

                Swal.fire(
                    'Deleted!',
                    'Your file has been deleted.',
                    'success'
                )
            }
        })
    })

    $(document).on('click','.deleteImage', function (e) {
        e.preventDefault();

        let url = $('.deleteImage').attr('href');
        let imageId = $(this).attr('data-imageId')
        console.log(url+" "+ imageId)

        fetch(url + "?imageId=" + imageId)
            .then(res => {
                if (res.ok) {
                    return res.text()
                } else {
                    alert("Yalnis Emeliyyat");
                    return
                }
            })
            .then(data => {
                $('.productImage').html(data)
            })
        
        console.log(url + "?imageId=" + imageId)
    })

    let IsMain = $('#IsMain').is(':checked');

    if (IsMain) {
        $('#fileInput').removeClass('d-none')
        $('#parentList').addClass('d-none')
    } else {
        $('#parentList').removeClass('d-none')
        $('#fileInput').addClass('d-none')

    }

    $('#IsMain').click(function () {
        let IsMain = $(this).is(':checked');

        if (IsMain) {
            $('#fileInput').removeClass('d-none')
            $('#parentList').addClass('d-none')
        } else {
            $('#parentList').removeClass('d-none')
            $('#fileInput').addClass('d-none')

        }
    })
})