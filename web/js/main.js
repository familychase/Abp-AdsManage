/**
 * 瑟文网络 - 官网交互脚本
 */

document.addEventListener('DOMContentLoaded', function () {

    // ----- 移动端导航菜单 -----
    var navToggle = document.getElementById('navToggle');
    var navLinks = document.getElementById('navLinks');

    if (navToggle && navLinks) {
        navToggle.addEventListener('click', function () {
            navToggle.classList.toggle('active');
            navLinks.classList.toggle('active');
        });

        // 点击菜单项后关闭菜单
        var links = navLinks.querySelectorAll('a');
        links.forEach(function (link) {
            link.addEventListener('click', function () {
                navToggle.classList.remove('active');
                navLinks.classList.remove('active');
            });
        });
    }

    // ----- 滚动时导航栏阴影 -----
    var header = document.querySelector('.header');
    if (header) {
        window.addEventListener('scroll', function () {
            if (window.scrollY > 10) {
                header.classList.add('scrolled');
            } else {
                header.classList.remove('scrolled');
            }
        });
    }

    // ----- 联系表单提交 -----
    var contactForm = document.getElementById('contactForm');
    var formSuccess = document.getElementById('formSuccess');

    if (contactForm && formSuccess) {
        contactForm.addEventListener('submit', function (e) {
            e.preventDefault();

            // 简单的表单验证
            var name = document.getElementById('name').value.trim();
            var email = document.getElementById('email').value.trim();
            var message = document.getElementById('message').value.trim();

            if (!name || !email || !message) {
                alert('请填写所有必填字段（姓名、邮箱、需求描述）。');
                return;
            }

            // 模拟提交（可替换为实际API调用）
            var submitBtn = contactForm.querySelector('button[type="submit"]');
            var originalText = submitBtn.textContent;
            submitBtn.textContent = '发送中...';
            submitBtn.disabled = true;

            setTimeout(function () {
                contactForm.style.display = 'none';
                formSuccess.style.display = 'block';
            }, 800);
        });
    }

    // ----- 滚动显示动画（Intersection Observer）-----
    if ('IntersectionObserver' in window) {
        var animatedElements = document.querySelectorAll('.feature-card, .service-card, .step, .value-item');
        var observerOptions = {
            threshold: 0.15,
            rootMargin: '0px 0px -30px 0px'
        };

        var observer = new IntersectionObserver(function (entries) {
            entries.forEach(function (entry) {
                if (entry.isIntersecting) {
                    entry.target.style.opacity = '1';
                    entry.target.style.transform = 'translateY(0)';
                    observer.unobserve(entry.target);
                }
            });
        }, observerOptions);

        animatedElements.forEach(function (el) {
            el.style.opacity = '0';
            el.style.transform = 'translateY(30px)';
            el.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
            observer.observe(el);
        });
    }

});
