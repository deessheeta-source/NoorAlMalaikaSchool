/* =========================================================
   Site.Master behaviours: sidebar/flyout + mobile card tables.
   Extracted from Site.Master (2026-10-29).
   ========================================================= */

    (function () {
        function initSidebar() {
            var sidebar = document.getElementById('sidebar');
            var toggle = document.getElementById('sidebarToggle');
            var overlay = document.getElementById('sidebarOverlay');

            if (!sidebar || !toggle || !overlay) return;

            function closeSidebar() {
                sidebar.classList.remove('show');
                overlay.classList.remove('show');
                toggle.setAttribute('aria-expanded', 'false');
                document.body.classList.remove('sidebar-open');
            }
            function openSidebar() {
                sidebar.classList.add('show');
                overlay.classList.add('show');
                toggle.setAttribute('aria-expanded', 'true');
                document.body.classList.add('sidebar-open');
            }

            toggle.addEventListener('click', function () {
                if (sidebar.classList.contains('show')) closeSidebar();
                else openSidebar();
            });
            overlay.addEventListener('click', closeSidebar);

            var flyoutItems = sidebar.querySelectorAll('.nav-item.has-flyout');

            function closeAllFlyouts(except) {
                sidebar.querySelectorAll('.nav-flyout.flyout-open').forEach(function (f) {
                    if (f !== except) f.classList.remove('flyout-open');
                });
            }

            function positionFlyout(item, flyout) {
                var parentLink = item.querySelector('.nav-link-parent');
                if (!parentLink) return;

                var parentRect = parentLink.getBoundingClientRect();

                var rightPos = window.innerWidth - parentRect.left + 8;
                var topPos = parentRect.top;

                var fh = flyout.scrollHeight || 200;

                if (topPos + fh > window.innerHeight - 12) {
                    topPos = window.innerHeight - fh - 12;
                }
                if (topPos < 12) topPos = 12;

                flyout.style.right = rightPos + 'px';
                flyout.style.top = topPos + 'px';
            }

            flyoutItems.forEach(function (item) {
                var flyout = item.querySelector('.nav-flyout');
                if (!flyout) return;

                var hideTimer = null;

                function showFlyout() {
                    if (window.innerWidth <= 991) return;
                    clearTimeout(hideTimer);
                    closeAllFlyouts(flyout);
                    flyout.classList.add('flyout-open');
                    positionFlyout(item, flyout);
                }

                function hideFlyout() {
                    if (window.innerWidth <= 991) return;
                    hideTimer = setTimeout(function () {
                        flyout.classList.remove('flyout-open');
                    }, 150);
                }

                item.addEventListener('mouseenter', showFlyout);
                item.addEventListener('mouseleave', hideFlyout);
                flyout.addEventListener('mouseenter', function () {
                    clearTimeout(hideTimer);
                });
                flyout.addEventListener('mouseleave', hideFlyout);
            });

            sidebar.addEventListener('click', function (event) {
                var parentLink = event.target.closest('.nav-item.has-flyout > .nav-link-parent');

                if (parentLink && window.innerWidth <= 991) {
                    event.preventDefault();
                    var item = parentLink.parentElement;
                    var flyout = item.querySelector('.nav-flyout');

                    sidebar.querySelectorAll('.nav-item.has-flyout.open').forEach(function (other) {
                        if (other !== item) {
                            other.classList.remove('open');
                            var otherFlyout = other.querySelector('.nav-flyout');
                            if (otherFlyout) otherFlyout.classList.remove('flyout-open');
                        }
                    });

                    item.classList.toggle('open');
                    if (flyout) flyout.classList.toggle('flyout-open');
                    return;
                }

                var link = event.target.closest('.nav-flyout a, .nav-item:not(.has-flyout) > .nav-link-parent');
                if (link && window.innerWidth <= 991) closeSidebar();
            });

            var navScroll = sidebar.querySelector('.sidebar-nav');
            if (navScroll) {
                navScroll.addEventListener('scroll', function () {
                    closeAllFlyouts(null);
                });
            }

            window.addEventListener('resize', function () {
                if (window.innerWidth > 991) {
                    closeSidebar();
                } else {
                    closeAllFlyouts(null);
                }
            });

            document.addEventListener('keydown', function (event) {
                if (event.key === 'Escape') {
                    closeSidebar();
                    closeAllFlyouts(null);
                }
            });
        }

        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', initSidebar);
        } else {
            initSidebar();
        }
    })();

/* ---- Mobile card tables ---- */

    (function () {
        function markMobileTableContainers(table) {
            var wrapper = table.closest('.table-responsive, [class*="table-wrap"]') || table.parentElement;
            if (wrapper) wrapper.classList.add('mobile-card-table-wrap');

            var cardBody = table.closest('.card-body, .card-body-custom, .fees-card-body, .sr-card-body, .id-card-body, .del-card-body');
            if (cardBody) cardBody.classList.add('mobile-card-table-container');
        }

        function getDirectHeaderRow(table) {
            if (table.tHead && table.tHead.rows.length) {
                return table.tHead.rows[table.tHead.rows.length - 1];
            }

            var bodies = Array.prototype.slice.call(table.tBodies || []);
            for (var bodyIndex = 0; bodyIndex < bodies.length; bodyIndex++) {
                var rows = Array.prototype.slice.call(bodies[bodyIndex].rows || []);
                for (var rowIndex = 0; rowIndex < rows.length; rowIndex++) {
                    var cells = Array.prototype.slice.call(rows[rowIndex].cells || []);
                    if (cells.some(function (cell) { return cell.tagName === 'TH'; }))
                        return rows[rowIndex];
                }
            }
            return null;
        }

        function enhanceMobileCardTables() {
            var pagePath = (window.location.pathname || '').toLowerCase();
            var tables = document.querySelectorAll('table');

            Array.prototype.forEach.call(tables, function (table) {
                if (table.hasAttribute('data-mobile-card-table')) return;
                if (table.closest('.no-mobile-cards')) return;

                // These pages already render and label their own mobile cards.
                if (table.classList.contains('us-table')
                    || table.classList.contains('attendance-table')
                    || table.classList.contains('take-table')
                    || (table.classList.contains('students-table')
                        && pagePath.endsWith('/pages/students/default.aspx'))) return;

                // Preserve tables that already provide their own mobile card markup.
                var hasExistingLabels = Array.prototype.some.call(table.tBodies || [], function (body) {
                    return Array.prototype.some.call(body.rows || [], function (row) {
                        return Array.prototype.some.call(row.cells || [], function (cell) {
                            return cell.hasAttribute('data-label');
                        });
                    });
                });
                if (hasExistingLabels) return;

                // Key/value and student summary tables do not have column headers.
                if (table.classList.contains('id-summary-table') || table.classList.contains('del-info-table')) {
                    table.setAttribute('data-mobile-card-table', 'true');
                    table.setAttribute('data-mobile-card-layout', 'key-value');
                    markMobileTableContainers(table);
                    return;
                }
                if (table.classList.contains('sr-info-table')) {
                    table.setAttribute('data-mobile-card-table', 'true');
                    table.setAttribute('data-mobile-card-layout', 'field-grid');
                    markMobileTableContainers(table);
                    return;
                }

                var headerRow = getDirectHeaderRow(table);
                if (!headerRow) return;

                var labels = Array.prototype.map.call(headerRow.cells, function (cell) {
                    return (cell.innerText || cell.textContent || '').replace(/\s+/g, ' ').trim();
                });
                if (!labels.some(function (label) { return label.length > 0; })) return;

                table.setAttribute('data-mobile-card-table', 'true');
                if (headerRow.parentElement && headerRow.parentElement.tagName === 'TBODY')
                    headerRow.classList.add('mobile-card-table-header');

                Array.prototype.forEach.call(table.tBodies || [], function (body) {
                    Array.prototype.forEach.call(body.rows || [], function (row) {
                        var cells = Array.prototype.slice.call(row.cells || []);
                        if (row === headerRow || cells.some(function (cell) { return cell.tagName === 'TH'; })) return;
                        if (cells.length === 1 && cells[0].colSpan > 1) return;

                        var columnIndex = 0;
                        cells.forEach(function (cell) {
                            var span = Math.max(1, cell.colSpan || 1);
                            var label = labels[columnIndex] || '';
                            if (label && !cell.hasAttribute('data-label'))
                                cell.setAttribute('data-label', label);
                            columnIndex += span;
                        });
                    });
                });

                Array.prototype.forEach.call(table.tFoot ? table.tFoot.rows : [], function (row) {
                    var columnIndex = 0;
                    Array.prototype.forEach.call(row.cells || [], function (cell) {
                        var span = Math.max(1, cell.colSpan || 1);
                        var label = labels[columnIndex] || '';
                        if (label && !cell.hasAttribute('data-label') && cell.colSpan === 1)
                            cell.setAttribute('data-label', label);
                        columnIndex += span;
                    });
                });

                markMobileTableContainers(table);
            });
        }

        enhanceMobileCardTables();
        if (window.Sys && Sys.Application)
            Sys.Application.add_load(enhanceMobileCardTables);
    })();