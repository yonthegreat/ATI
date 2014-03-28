using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using System.Security.Principal;

namespace AtsAPCC.Tests.MvcFakes
{

    public class FakeControllerContext : ControllerContext
    {
        public FakeControllerContext(ControllerBase controller)
            : this(controller, null, null, null, null, null)
        {

        }

        public FakeControllerContext(ControllerBase controller, SessionStateItemCollection sessionItems)
            : this(controller, null, null, null, null, sessionItems)
        {
        }


        public FakeControllerContext(ControllerBase controller, NameValueCollection formParams)
            : this(controller, null, formParams, null, null, null)
        {
        }


        public FakeControllerContext(ControllerBase controller, IPrincipal user)
            : this(controller, user, new NameValueCollection(), new NameValueCollection(), null, null)
        {
        }

        public FakeControllerContext(ControllerBase controller, IPrincipal user, SessionStateItemCollection sessionItems)
            : this(controller, user, null, null, null, sessionItems)
        {
        }

        public FakeControllerContext
        (
            ControllerBase controller,
            IPrincipal user,
            NameValueCollection formParams,
            NameValueCollection queryStringParams,
            HttpCookieCollection cookies,
            SessionStateItemCollection sessionItems
        )
            : base(new FakeHttpContext(user, formParams, queryStringParams, cookies, sessionItems), new RouteData(), controller)
        { }
    }
}