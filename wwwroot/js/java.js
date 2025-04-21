// java.js - Xử lý chung cho toàn bộ trang web
$(document).ready(function () {
    // scroll cho navbar
    var prevScrollpos = window.pageYOffset;
    window.onscroll = function () {
        var currentScrollPos = window.pageYOffset;
        if (prevScrollpos > currentScrollPos) {
            document.getElementById("navbar").style.top = "0";
        } else {
            document.getElementById("navbar").style.top = "-60px";
        }
        prevScrollpos = currentScrollPos;
    };

    // click cho các nút trong trang
    setupButtonHandlers();

    // click cho các project card
    setupProjectCards();

    // click cho các thành viên
    setupTeamMembers();

    // Xử lý cho nút đăng xuất
    $('#logout-btn').on('click', function (e) {
        e.preventDefault();
        $('#logoutForm').submit();
    });

    // Thiết lập slider header ngay khi trang tải xong
    setupHeaderSlider();
});

function setupButtonHandlers() {
    console.log("Setting up button handlers");

    // nút đăng nhập
    $('#login-btn').on('click', function (e) {
        e.preventDefault();
        console.log("Login button clicked");
        $('#loginModal').css('display', 'block');
    });

    // nút đăng ký
    $('#register-btn').on('click', function (e) {
        e.preventDefault();
        console.log("Register button clicked");
        $('#registerModal').css('display', 'block');
    });

    // form đăng nhập
    $('#loginForm').on('submit', function (e) {
        e.preventDefault();

        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            success: function (result) {
                if (result.success === false) {
                    $('#login-error').text(result.message);
                } else {
                    window.location.reload();
                }
            },
            error: function () {
                $('#login-error').text('Có lỗi xảy ra. Vui lòng thử lại sau.');
            }
        });
    });

    // form đăng ký
    $('#registerForm').on('submit', function (e) {
        e.preventDefault();

        if ($('#regPassword').val() !== $('#confirmPassword').val()) {
            $('#register-error').text('Mật khẩu xác nhận không khớp.');
            return;
        }

        $.ajax({
            url: this.action,
            type: this.method,
            data: $(this).serialize(),
            success: function (result) {
                if (result.success === false) {
                    $('#register-error').text(result.message || 'Đăng ký không thành công.');

                    if (result.errors && result.errors.length > 0) {
                        $('#register-error').text(result.errors.join(', '));
                    }
                } else {
                    window.location.reload();
                }
            },
            error: function () {
                $('#register-error').text('Có lỗi xảy ra. Vui lòng thử lại sau.');
            }
        });
    });

    // Kiểm tra email đã tồn tại khi nhập
    $('#regEmail').on('blur', function () {
        const email = $(this).val();
        if (email) {
            $.ajax({
                url: '/Account/CheckEmailExists',
                type: 'GET',
                data: { email: email },
                success: function (result) {
                    if (!result) {
                        $('#register-error').text('Email này đã được sử dụng.');
                    } else {
                        $('#register-error').text('');
                    }
                }
            });
        }
    });

    // Đóng modal 
    $('.close').on('click', function () {
        $('.modal').css('display', 'none');
    });

    // Đóng modal
    $(window).on('click', function (event) {
        $('.modal').each(function () {
            if (event.target == this) {
                $(this).css('display', 'none');
            }
        });
    });
}

function setupProjectCards() {
    console.log("Setting up project cards");

    // click các project card
    $('.project-card').on('click', function () {
        var projectId = $(this).data('id');
        console.log("Project card clicked, ID:", projectId);

        // Gọi API để lấy thông tin chi tiết dự án
        $.ajax({
            url: '/Home/GetProjectDetails/' + projectId,
            type: 'GET',
            success: function (data) {
                // Điền thông tin vào modal
                $('#project-title').text(data.tenProject);
                $('#project-image').attr('src', '/' + data.linkAnh);
                $('#project-location').text('Địa điểm: ' + (data.diaDiem || 'Chưa cập nhật'));
                $('#project-year').text('Năm hoàn thành: ' + (data.namHoanThanh || 'Chưa cập nhật'));
                $('#project-description').text(data.moTaChiTiet || 'Chưa có thông tin chi tiết.');

                // Hiển thị modal
                $('#projectModal').css('display', 'block');
            },
            error: function (error) {
                console.error("Error fetching project details:", error);
            }
        });
    });
}

function setupTeamMembers() {
    console.log("Setting up team members");

    //  click các thành viên
    $('.team-member').on('click', function () {
        var memberId = $(this).data('id');
        console.log("Team member clicked, ID:", memberId);

        // Gọi API để lấy thông tin chi tiết thành viên
        $.ajax({
            url: '/Home/GetTeamMemberDetails/' + memberId,
            type: 'GET',
            success: function (data) {
                // Điền thông tin vào modal
                $('#team-name').text(data.name);
                $('#team-image').attr('src', '/' + data.image);
                $('#team-position').text(data.position);
                $('#team-bio').text(data.bio || 'Chưa có thông tin chi tiết.');
                $('#team-email').text(data.email || 'Chưa cập nhật');
                $('#team-phone').text(data.phone || 'Chưa cập nhật');

                // Hiển thị modal
                $('#teamModal').css('display', 'block');
            },
            error: function (error) {
                console.error("Error fetching team member details:", error);
            }
        });
    });
}

// Hàm slider cho header
function setupHeaderSlider() {
    console.log("Setting up header slider");

    const images = [
        'images/architect.jpg',
        'images/architect1.jpg',
        'images/architect2.jpg',
        'images/architect3.jpg'
    ];

    let currentImageIndex = 0;
    const headerImage = $('.header-image');

    // Nếu có hình ảnh header
    if (headerImage.length) {
        console.log("Header image found, setting up slider");

        // Tạo chức năng thay đổi hình ảnh
        function changeHeaderImage() {
            currentImageIndex = (currentImageIndex + 1) % images.length;

            // Tạo hiệu ứng fade
            headerImage.css('opacity', 0);

            setTimeout(function () {
                headerImage.attr('src', images[currentImageIndex]);
                headerImage.css('opacity', 1);
            }, 500);
        }

        // Thiết lập interval để thay đổi hình ảnh sau mỗi 5 giây
        setInterval(changeHeaderImage, 5000);
    } else {
        console.log("Header image not found");
    }
}

function logElementsOnPage() {
    console.log("Login button exists:", $('#login-btn').length > 0);
    console.log("Register button exists:", $('#register-btn').length > 0);
    console.log("Login modal exists:", $('#loginModal').length > 0);
    console.log("Register modal exists:", $('#registerModal').length > 0);
    console.log("Project cards exist:", $('.project-card').length > 0);
    console.log("Team members exist:", $('.team-member').length > 0);
    console.log("Header image exists:", $('.header-image').length > 0);
}

$(document).ready(function () {
    logElementsOnPage();
});