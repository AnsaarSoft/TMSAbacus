<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportViewer.aspx.cs" Inherits="TMSDeloitte.ReportViewer" %>

<%@ Register Assembly="CrystalDecisions.Web, Version=13.0.3500.0, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>ReportViewer</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, user-scalable=0, minimal-ui"/>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="description" content="" />
    <meta name="keywords" content=""/>
    <meta name="author" content="Phoenixcoded" />

    <!-- font css -->

  
    <link href="Content/bootstrap.css" rel="stylesheet" />
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <link href="assets/css/customizer.css" rel="stylesheet" />
    <link href="assets/css/style.css" rel="stylesheet" />
    
        <!-- font css -->
    <link rel="stylesheet" href="assets/fonts/font-awsome-pro/css/pro.min.css"/>
    <link rel="stylesheet" href="assets/fonts/feather.css"/>
    <link rel="stylesheet" href="assets/fonts/fontawesome.css"/>
    


    <script src="assets/js/vendor-all.min.js"></script>
    <script src="assets/js/plugins/feather.min.js"></script>
    <script src="assets/js/pcoded.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/highlight.js/10.3.1/highlight.min.js"></script>
    <script src="assets/js/plugins/clipboard.min.js"></script>
    <script src="assets/js/uikit.min.js"></script>
    <script src="assets/js/main.js"></script>

</head>

    <body class="modern-layout">
    <!-- [ Pre-loader ] start -->
    <div class="loader-bg">
        <div class="loader-track">
            <div class="loader-fill"></div>
        </div>
    </div>
    <!-- [ Pre-loader ] End -->
    <!-- [ Mobile header ] start -->
    <div class="pc-mob-header pc-header">
        <div class="pcm-logo">
            <img src="assets/images/Logo/innerLogo.png" />
        
        </div>
        <div class="pcm-toolbar">
            <a href="#!" class="pc-head-link" id="mobile-collapse">
                <div class="hamburger hamburger--arrowturn">
                    <div class="hamburger-box">
                        <div class="hamburger-inner"></div>
                    </div>
                </div>
            </a>
            <!-- <a href="#!" class="pc-head-link" id="headerdrp-collapse">
                <i data-feather="align-right"></i>
            </a> -->
            <a href="#!" class="pc-head-link" id="header-collapse">
                <i data-feather="more-vertical"></i>
            </a>
        </div>
    </div>
    <!-- [ Mobile header ] End -->
    <!-- [ navigation menu ] start -->
    <nav class="pc-sidebar light-sidebar">
        <div class="navbar-wrapper">
            <div class="navbar-content">
                <ul class="pc-navbar">
                    <li class="pc-item">
                        <a href="/Dashboard/Index" class="pc-link "><span class="pc-micon"><i class="fas fa-tachometer-alt"></i></span><span class="pc-mtext">Dashboard</span></a>
                    </li>

                   <%{ 

                        Dictionary<int, string> dictLinkClass = new Dictionary<int, string>();
                        dictLinkClass.Add(1, "fas fa-user-shield");
                        dictLinkClass.Add(2, "fas fa-clock");
                        dictLinkClass.Add(3, "fas fa-plane-departure");
                        dictLinkClass.Add(4, "fas fa-copy");
                        //string userIDFromDictionaryByKey = dictLinkClass["UserID"];

                        TMSDeloitte.Models.UserSession sess = null;
                        if ((TMSDeloitte.Models.UserSession)HttpContext.Current.Session["TMSUserSession"] != null)
                        {
                            sess = (TMSDeloitte.Models.UserSession)HttpContext.Current.Session["TMSUserSession"];%>

                            <%foreach (var parentMenu in sess.pagelist.Where(x=>x.ID==1 || x.ID==2 || x.ID==3 || x.ID==4).ToList())
                            {
                                string className = dictLinkClass[parentMenu.ID];%>
                                  <li class="pc-item pc-hasmenu">
                                      <a href="#!" class="pc-link"><span class="pc-micon"><i class="<%=className%>"></i></span><span class="pc-mtext"> <%=parentMenu.PageName%> </span><span class="pc-arrow"><i data-feather="chevron-right"></i></span></a>
                                      <ul class="pc-submenu">
                                          <%{

                                              foreach (var childMenu in sess.pagelist.Where(x => x.HeadID == parentMenu.ID).ToList())
                                              {
                                                  var subChildMenuList = sess.pagelist.Where(x => x.HeadID == childMenu.ID).ToList();
                                                  if (subChildMenuList.Count==0)
                                                  {%>
                                                    <li class="pc-item"><a class="pc-link" href="<%=childMenu.PageURL %>"><%=childMenu.PageName%> </a></li>
                                                  <%}
                                                  else
                                                  {%>
                                                   <li class="pc-item pc-hasmenu">
                                                       <a href="#" class="pc-link"><%=childMenu.PageName%> <span class="pc-arrow"><i data-feather="chevron-right"></i></span> </a>
                                                    <ul class="pc-submenu">
                                                        <%{
                                                            foreach (var subChild in subChildMenuList)
                                                            {%>
                                                                <li class="pc-item"><a class="pc-link" href="<%=subChild.PageURL%>"><%=subChild.PageName%> </a></li>
                                                            <%}
                                                        }%>
                                                    </ul>
                                                    </li>
                                                  <%}

                                              }

                                        }%>
                                      </ul>
                                   </li>
                            <%}
                        }
                    }%>
                </ul>
            </div>
        </div>
    </nav>
    <!-- [ navigation menu ] end -->
    <!-- [ Header ] start -->


    <header class="pc-header bg-white">
        <div class="header-wrapper">
            
            <div id="menu-collapse" class="menu-btn">
                <a href="#"> <i class="fas fa-bars"></i> </a>
            </div>
            
            <div class="m-header d-flex align-items-center">
                <a href="/Dashboard/Index" class="b-brand">
                    <!-- ========   change your logo hear   ============ -->
                    <img src="assets/images/Logo/innerLogo.png" alt="" class="logo logo-lg"/>
                </a>
            </div>

            <div class="ml-auto">
                <ul class="list-unstyled">
                    <li class="dropdown pc-h-item pc-cart-menu">
                        <a class="pc-head-link dropdown-toggle arrow-none mr-0" data-toggle="dropdown" href="#" role="button" aria-haspopup="false" aria-expanded="false">
                            <i data-feather="bell"></i>
                            <span class="badge badge-danger pc-h-badge dots" id="bell" ><span class="sr-only"></span></span>
                        </a>
                        <div class="dropdown-menu dropdown-menu-right pc-h-dropdown drp-cart" id="NotifiCationItems">
                            
                        </div>
                    </li>

                    <li class="dropdown pc-h-item">
                        <a class="pc-head-link dropdown-toggle arrow-none mr-0" data-toggle="dropdown" href="#" role="button" aria-haspopup="false" aria-expanded="false">
                            <img src="assets/images/avatar-2.jpg" alt="user-image" class="user-avtar"/>
                            <span>
                                <%{
                                        TMSDeloitte.Models.UserSession sess = null;
                                        if ((TMSDeloitte.Models.UserSession)HttpContext.Current.Session["TMSUserSession"] != null)
                                        {
                                            sess = (TMSDeloitte.Models.UserSession)HttpContext.Current.Session["TMSUserSession"];
                                            int uID = 0;
                                            if (sess != null)
                                            {
                                                uID = sess.SessionUser.ID;
                                            }%>
                                            
                                             <input type="hidden" id="userID"  value="<%=uID%>" />

                                            <% if (sess != null)
                                            {%>
                                           
                                            <span class="user-name"> <%=sess.SessionUser.FULLNAME%> </span>
                                            <span class="user-desc"> <%=sess.SessionUser.DESIGNATIONNAME%> </span>
                                            <%}
                                            
                                        }%>
                               
                                    </span>
                        </a>
                        <div class="dropdown-menu dropdown-menu-right pc-h-dropdown">
                            <%{
                                if (sess != null)
                                {
                                    if (sess.SessionUser.ISSUPER)
                                    {%>
                                        <a href="/Home/UploadLogo" class="dropdown-item">
                                            <i data-feather="upload"></i>
                                            <span> Upload Logo </span>
                                        </a>
                                    <%}
                                }

                            }%>
                           
                            <a href="/Home/UpdatePassword" class="dropdown-item">
                                <i data-feather="lock"></i>
                                <span> Change Password </span>
                            </a>
                            <%{
                                if (sess != null)
                                {%>
                                    <a href="<%=sess.SessionUser.HCMONELINK%>" target="_blank" class="dropdown-item">
                                        <i data-feather="life-buoy"></i>
                                        <span> HCM One Link </span>
                                    </a>
                                <%}
                              }
                         }%>
                            <a href="/Home/Logout" class="dropdown-item">
                                <i data-feather="power"></i>
                                <span>Logout</span>
                            </a>
                        </div>
                    </li>
                </ul>
            </div>

        </div>
    </header>

    <!-- Modal -->
    <div class="modal notification-modal fade" id="notification-modal" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-body">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                    <ul class="nav nav-pill tabs-light mb-3" id="pc-noti-tab" role="tablist">
                        <li class="nav-item">
                            <a class="nav-link active" id="pc-noti-home-tab" data-toggle="pill" href="#pc-noti-home" role="tab" aria-controls="pc-noti-home" aria-selected="true">Notification</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="pc-noti-news-tab" data-toggle="pill" href="#pc-noti-news" role="tab" aria-controls="pc-noti-news" aria-selected="false">News<span class="badge badge-danger ml-2 d-none d-sm-inline-block">4</span></a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" id="pc-noti-settings-tab" data-toggle="pill" href="#pc-noti-settings" role="tab" aria-controls="pc-noti-settings" aria-selected="false">Setting<span class="badge badge-success ml-2 d-none d-sm-inline-block">Update</span></a>
                        </li>
                    </ul>
                    <div class="tab-content pt-4" id="pc-noti-tabContent">
                        <div class="tab-pane fade show active" id="pc-noti-home" role="tabpanel" aria-labelledby="pc-noti-home-tab">
                            <div class="media">
                                <img src="assets/images/user/avatar-1.jpg" alt="images" class="img-fluid avtar avtar-l"/>
                                <div class="media-body ml-3 align-self-center">
                                    <div class="float-right">
                                        <div class="btn-group card-option">
                                            <button type="button" class="btn shadow-none">
                                                <i data-feather="heart" class="text-danger"></i>
                                            </button>
                                            <button type="button" class="btn shadow-none px-0 dropdown-toggle arrow-none" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                                <i data-feather="more-horizontal"></i>
                                            </button>
                                            <div class="dropdown dropdown-menu dropdown-menu-right">
                                                <a class="dropdown-item" href="#!"><i data-feather="refresh-cw"></i> reload</a>
                                                <a class="dropdown-item" href="#!"><i data-feather="trash"></i> remove</a>
                                            </div>
                                        </div>
                                    </div>
                                    <h6 class="mb-0 d-inline-block">Ashoka T.</h6>
                                    <p class="mb-0 d-inline-block"> • 06/20/2019 at 6:43 PM </p>
                                    <p class="my-3">Cras sit amet nibh libero in gravida nulla Nulla vel metus scelerisque ante sollicitudin.</p>
                                    <div class="p-3 mb-3 border rounded">
                                        <div class="media align-items-center">
                                            <div class="media-body">
                                                <h6 class="mb-1">Death Star original maps and blueprint.pdf</h6>
                                                <p class="mb-0">by Ashoka T. • 06/20/2019 at 6:43 PM </p>
                                            </div>
                                            <div class="btn-group d-none d-sm-inline-flex">
                                                <button type="button" class="btn shadow-none">
                                                    <i data-feather="download-cloud"></i>
                                                </button>
                                                <button type="button" class="btn shadow-none px-0 dropdown-toggle arrow-none" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                                    <i data-feather="more-horizontal"></i>
                                                </button>
                                                <div class="dropdown dropdown-menu dropdown-menu-right">
                                                    <a class="dropdown-item" href="#!"><i data-feather="refresh-cw"></i> reload</a>
                                                    <a class="dropdown-item" href="#!"><i data-feather="trash"></i> remove</a>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <hr class="mb-4" />
                            <div class="media">
                                <img src="assets/images/user/avatar-2.jpg" alt="images" class="img-fluid avtar avtar-l"/>
                                <div class="media-body ml-3 align-self-center">
                                    <div class="float-right">
                                        <div class="btn-group card-option">
                                            <button type="button" class="btn shadow-none px-0 dropdown-toggle arrow-none" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                                <i data-feather="more-horizontal"></i>
                                            </button>
                                            <div class="dropdown dropdown-menu dropdown-menu-right">
                                                <a class="dropdown-item" href="#!"><i data-feather="refresh-cw"></i> reload</a>
                                                <a class="dropdown-item" href="#!"><i data-feather="trash"></i> remove</a>
                                            </div>
                                        </div>
                                    </div>
                                    <h6 class="mb-0 d-inline-block">Ashoka T.</h6>
                                    <p class="mb-0 d-inline-block"> • 06/20/2019 at 6:43 PM </p>
                                    <p class="my-3">Cras sit amet nibh libero in gravida nulla Nulla vel metus scelerisque ante sollicitudin.</p>
                                    <img src="assets/images/slider/img-slide-3.jpg" alt="images" class="img-fluid wid-90 rounded m-r-10 m-b-10" />
                                    <img src="assets/images/slider/img-slide-7.jpg" alt="images" class="img-fluid wid-90 rounded m-r-10 m-b-10" />
                                </div>
                            </div>
                            <hr class="mb-4" />
                            <div class="media mb-3">
                                <img src="assets/images/user/avatar-3.jpg" alt="images" class="img-fluid avtar avtar-l" />
                                <div class="media-body ml-3 align-self-center">
                                    <div class="float-right">
                                        3 <i data-feather="heart" class="text-danger"></i>
                                    </div>
                                    <h6 class="mb-0 d-inline-block">Ashoka T.</h6>
                                    <p class="mb-0 d-inline-block"> • 06/20/2019 at 6:43 PM </p>
                                    <p class="my-3">Nulla vitae elit libero, a pharetra augue. Aenean lacinia bibendum nulla sed consectetur.</p>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane fade" id="pc-noti-news" role="tabpanel" aria-labelledby="pc-noti-news-tab">
                            <div class="pb-3 border-bottom mb-3 media">
                                <a href="#!"><img src="assets/images/news/img-news-2.jpg" class="wid-90 rounded" alt="..."/></a>
                                <div class="media-body ml-3">
                                    <p class="float-right mb-0 text-success"><small>now</small></p>
                                    <a href="#!"><h6>This is a news image</h6></a>
                                    <p class="mb-2">Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy.</p>
                                </div>
                            </div>
                            <div class="pb-3 border-bottom mb-3 media">
                                <a href="#!"><img src="assets/images/news/img-news-1.jpg" class="wid-90 rounded" alt="..."/></a>
                                <div class="media-body ml-3">
                                    <p class="float-right mb-0 text-muted"><small>3 mins ago</small></p>
                                    <a href="#!"><h6>Industry's standard dummy</h6></a>
                                    <p class="mb-2">Lorem Ipsum is simply dummy text of the printing and typesetting.</p>
                                    <a href="#" class="badge badge-light">Html</a>
                                    <a href="#" class="badge badge-light">UI/UX designed</a>
                                </div>
                            </div>
                            <div class="pb-3 border-bottom mb-3 media">
                                <a href="#!"><img src="assets/images/news/img-news-2.jpg" class="wid-90 rounded" alt="..." /></a>
                                <div class="media-body ml-3">
                                    <p class="float-right mb-0 text-muted"><small>5 mins ago</small></p>
                                    <a href="#!"><h6>Ipsum has been the industry's</h6></a>
                                    <p class="mb-2">Lorem Ipsum is simply dummy text of the printing and typesetting.</p>
                                    <a href="#" class="badge badge-light">JavaScript</a>
                                    <a href="#" class="badge badge-light">Scss</a>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane fade" id="pc-noti-settings" role="tabpanel" aria-labelledby="pc-noti-settings-tab">
                            <h6 class="mt-2"><i data-feather="monitor" class="mr-2"></i>Desktop settings</h6>
                            <hr/>
                            <div class="custom-control custom-switch">
                                <input type="checkbox" class="custom-control-input" id="pcsetting1" checked="checked"/>
                                <label class="custom-control-label f-w-600 pl-1" for="pcsetting1">Allow desktop notification</label>
                            </div>
                            <p class="text-muted ml-5">you get lettest content at a time when data will updated</p>
                            <div class="custom-control custom-switch">
                                <input type="checkbox" class="custom-control-input" id="pcsetting2"/>
                                <label class="custom-control-label f-w-600 pl-1" for="pcsetting2">Store Cookie</label>
                            </div>
                            <h6 class="mb-0 mt-5"><i data-feather="save" class="mr-2"></i>Application settings</h6>
                            <hr />
                            <div class="custom-control custom-switch">
                                <input type="checkbox" class="custom-control-input" id="pcsetting3"/>
                                <label class="custom-control-label f-w-600 pl-1" for="pcsetting3">Backup Storage</label>
                            </div>
                            <p class="text-muted mb-4 ml-5">Automaticaly take backup as par schedule</p>
                            <div class="custom-control custom-switch">
                                <input type="checkbox" class="custom-control-input" id="pcsetting4"/>
                                <label class="custom-control-label f-w-600 pl-1" for="pcsetting4">Allow guest to print file</label>
                            </div>
                            <h6 class="mb-0 mt-5"><i data-feather="cpu" class="mr-2"></i>System settings</h6>
                            <hr />
                            <div class="custom-control custom-switch">
                                <input type="checkbox" class="custom-control-input" id="pcsetting5" checked="checked"/>
                                <label class="custom-control-label f-w-600 pl-1" for="pcsetting5">View other user chat</label>
                            </div>
                            <p class="text-muted ml-5">Allow to show public user message</p>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-light-danger btn-sm" data-dismiss="modal">Close</button>
                    <button type="button" class="btn btn-light-primary btn-sm">Save changes</button>
                </div>
            </div>
        </div>
    </div>
    <!-- [ Header ] end -->
    <!-- [ Main Content ] start -->
    <div class="pc-container">
   
    <form id="form1" runat="server">
    <div>
        
        <div class="pcoded-content">
    <div class="card p-20" style="min-height: 495px;">
        <!-- [ breadcrumb ] start -->
        <div class="page-header">
            <div class="page-block">
                <div class="row align-items-center">
                    <div class="col-md-6">
                        <div class="page-header-title">
                            <h4> Report Viewer </h4>
                        </div>
                    </div>
                    <div class="col-md-6 text-md-right">
                        <ul class="breadcrumb">
                            <li class="breadcrumb-item"><a href="#"> Reports </a></li>
                            <li class="breadcrumb-item"> Report Viewer </li>
                        </ul>
                    </div>
                </div>
            </div>
        </div>
        <!-- [ breadcrumb ] end -->
        <!-- [ Main Content ] start -->
        <%--<div class="row">
            <div id="mvcpartial" style="width:600px;height:400px"></div>
        </div>--%>
        <CR:CrystalReportViewer ID="CRV" runat="server" AutoDataBind="true" />
        
            <!-- [ Main Content ] end -->
        </div> <!-- [ Card Content ] end -->
    </div>
    </div>
    </form>
    </div>
    <!-- [ Main Content ] end -->
    <div class="loadpanel"></div>
    <div id="myModal" class="modal fade" role="dialog">
        <div class="modal-dialog modal-lg">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <%--<h4 class="modal-title">Log</h4>--%>
                    <button type="button" class="close" data-dismiss="modal">×</button>

                </div>
                <div class="modal-body">
                    <div id="container"></div>
                </div>
               
            </div>
        </div>
    </div>


</body>

</html>

<script type="text/javascript">
    var loadPanel = $(".loadpanel").dxLoadPanel({
        shadingColor: "rgba(0,0,0,0.4)",
        visible: false,
        showIndicator: true,
        showPane: true,
        shading: true,
        closeOnOutsideClick: false,
        onShown: function () {
        },
        onHidden: function () {
        }
    }).dxLoadPanel("instance");

    $(document).ready(function () {

        $("#bell").hide();

        GetNotifications();
        setInterval(function () {
            //console.log('GetNotifications');
            GetNotifications();
        }, 1000 * 300);  //1000 -> 10 Sec
       

        $(".pc-sidebar .pc-navbar a").each(function () {
            var e = window.location.href.split(/[?#]/)[0];
            this.href == e &&
                "" != $(this).attr("href") &&
                ($(this).addClass("active-page-link"),
                $(this).parent("li").addClass("active"),
                $(this).parent("li").parent().parent(".pc-hasmenu").addClass("active").addClass("pc-trigger"),
                $(this).parent("li").parent().parent(".sidelink").addClass("active"),
                $(this).parent("li").parent().parent(".pc-hasmenu").parent().parent(".pc-hasmenu").addClass("active").addClass("pc-trigger"),
                $(this).parents(".pc-tabcontent").addClass("active"));
        });


    });
   
    function GetNotifications()
    {
        $("#NotifiCationItems").html('');

        if ($("#userID").val() == 0)
            return;

        var url = "/Notification/GetNotifications?id=" + $("#userID").val();
        $.ajax({
            url: url,
            method: "GET",
            data: {},
        }).done(function (data) {
           

            $.each(data.NotificationList, function (index, val) {
                var detail = "Click to view details";
                if (val.Table == false)
                {
                    detail = val.Detail
                }
                var html = '<div class="cart-item">' +
                               ' <div class="cart-desc">' +
                               '   <a href="/Notification/Detail?NotificationID=' + val.ID + '" class="text-body">' +
                               '     <h6 class="d-inline-block mb-0">' + val.FromEmp + '</h6>' +
                               '   </a>' +
                               '   <p class="mb-0">' + detail + '</p>' +
                               '   <p class="mb-0">' + val.Date  + '</p>' +
                               '  </div>' +
                               ' </div>';
                $("#NotifiCationItems").append(html);
            });

            if (data.NotificationList.length > 0)
            {
                $("#bell").show();
            }
               
            else
            {
                $("#bell").hide();

                var html = '<div class="cart-item">' +
                             ' <div class="cart-desc">' +
                             '   <a href="#" class="text-body">' +
                             '     <h6 class="d-inline-block mb-0">There is no any unread notification</h6>' +
                             '   </a>' +
                             '   <p class="mb-0"></p>' +
                             '   <p class="mb-0"></p>' +
                             '  </div>' +
                             ' </div>';
                $("#NotifiCationItems").append(html);
            }
               
            var html = '<div class="cart-item" style="background: #179bd2;">' +
                          ' <div class="cart-desc">' +
                          '   <a href="/Notification/Index" class="text-body">' +
                          '     <h6 class="d-inline-block mb-0">Click To View All Notifications</h6>' +
                          '   </a>' +
                          '   <p class="mb-0"></p>' +
                          '   <p class="mb-0"></p>' +
                          '  </div>' +
                          ' </div>';
            $("#NotifiCationItems").append(html);

        }).fail(function (data) {
            console.log("Exception at GetNotifications method is :" + data);
        });
}

    
    function loadData(url) {
        $.ajax({
            url: url,
            method: "GET",
            data: {},
            async: false,
        }).done(function (data) {
            var JSON_Response = JSON.parse(data.response.Data);
            $("#container").dxDataGrid({
                dataSource: JSON_Response,
                paging: { pageSize: 10 },
                columnAutoWidth: true,
                pager: { showInfo: true },
                allowColumnResizing: true,
                allowColumnReordering: true,
                showBorders: true,
                showScrollbar: 'always',
                filterRow: { visible: true },
                scrolling: {
                    mode: "horizontal",
                    showScrollbar: 'always'
                },
            });
        }).fail(function (data) {
        });

    }

    
</script>

