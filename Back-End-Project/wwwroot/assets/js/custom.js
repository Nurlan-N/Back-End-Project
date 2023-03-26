$(document).ready(function () {
    $('.accordian-body').on('show.bs.collapse', function () {
        $(this).closest("table")
            .find(".collapse.in")
            .not(this)
            .collapse('toggle')
    })
    $(document).on('click' ,'.plush' ,function () {
        var countInput = $(this).siblings(".prCount");
        var count = parseInt(countInput.val());
        let productId = $(this).attr('data-id');
        count++;
        countInput.val(count);
        console.log(count + " " + productId);
        fetch('basket/ChangeCount?id=' + productId + '&count=' + count)
            .then(res => {
                return res.text()
            }).then(data => {
                $('.basketContainer').html(data)
            })
    });

    $(document).on('click','.minush', function () {
        var countInput = $(this).siblings(".prCount");
        var count = parseInt(countInput.val());
        let productId = $(this).attr('data-id');
        if (count > 1) {
            count--;
            countInput.val(count);
        }
        fetch('basket/ChangeCount?id=' + productId + '&count=' + count)
            .then(res => {
                return res.text()
            }).then(data => {
                $('.basketContainer').html(data)
            })
    });
    $(document).on('click', '.deleteWishlist', function (e) {
        e.preventDefault();

        const removeId = $(this).attr('data-id');
        fetch('wishlist/DeleteWishlist?id=' + removeId)
            .then(res => {
                return res.text();
            })
        window.location.reload();


    })
    $('.addToWishlist').click(function myFunction(e) {
        e.preventDefault();

        let productId = $(this).data('id');
        fetch('Wishlist/AddWishlist?id=' + productId)
            .then(res => {
                return res.text();
            })

    })
    $(document).on('click', '.addAddress', function (e) {
        e.preventDefault();

        $('.addressContainer').addClass('d-none');
        $('.addressForm').removeClass('d-none')
    })

 
    $(document).on('change', 'select[name=sortby]', function () {
        let categoryName = $('.categoryName').attr('href')
        let categoryId = $(this).attr('data-categoryId');
        let pageIndex = $(this).attr('data-pageIndex');
        let sort = $(this).val();

        let url = '/shop/Index?categoryId=' + categoryId + "&sort=" + sort + "&pageIndex=" + pageIndex;

        window.location.href = url;
        let ss = window.location.pathname.split("&").split("=")[1];
        console.log(ss);
        fetch(url)
            .then(res => res.text())
            .then(data => {
                $(this).val(ss);
            })
    })

    $('.rangeFilter').click(function (e) {
        e.preventDefault();

        const val = $('.rangeInput').val();

        fetch('shop/list?range=' + val)
            .then(res => {
                return res.text();
            })
            .then(data => {
                $('.shopList').html(data)
            })
    })

    $(document).on('click', '.addToBasket',function myFunction(e) {
        e.preventDefault();

        let productId = $(this).data('id');
        console.log(productId)
        fetch('basket/AddBasket?id=' + productId)
            .then(res => {
                return res.text();
            }).then(data => {
                $('.header-cart').html(data)
                console.log("ok")

                $(".offcanvas-close, .minicart-close,.offcanvas-overlay").on('click', function () {
                    $("body").removeClass('fix');
                    $(".offcanvas-search-inner, .minicart-inner").removeClass('show')
                })
            })
            

    })
    $(document).on('click', '.deleteToBasket', function (e) {
        e.preventDefault();

        const removeId = $(this).attr('data-id');

        fetch('basket/DeleteBasket?id=' + removeId)
            .then(res => {
                return res.text();
            }).then(data => {
                $('.header-cart').html(data)
                $(".minicart-inner").addClass('show')
                $(".offcanvas-close, .minicart-close,.offcanvas-overlay").on('click', function () {
                    $("body").removeClass('fix');
                    $(".offcanvas-search-inner, .minicart-inner").removeClass('show')
                })
            })

    })
    $('#SearchValue').keyup(function () {
        let search = $(this).val();
        console.log(search)
        if (search.Trim().length >= 3) {
            fetch('product/search?search=' + search)
                .then(res => {
                    return res.text()
                }).then(data => {
                    $('.searchBody').html(data)
                })
        } else {
            $('.searchBody').html('')

        }
    })


    $(".productModal").click(function (e) {
        e.preventDefault();

        let url = $(this).attr('href')

        fetch(url).then(res => {
            return res.text();
        })
            .then(data => {
                $('.modal-content').html(data)
                // prodct details slider active
                $('.product-large-slider').slick({
                    fade: true,
                    arrows: false,
                    asNavFor: '.pro-nav'
                });


                // product details slider nav active
                $('.pro-nav').slick({
                    slidesToShow: 4,
                    asNavFor: '.product-large-slider',
                    arrows: false,
                    focusOnSelect: true
                });

            })
    })
})
