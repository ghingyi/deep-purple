<h1>
    Welcome to Deep Purple!
</h1>
<h2>
    Introduction
</h2>
<p>
    I built this solution as part of my application for a job, in order to show evidence of a good understanding of software design principles and the
    capability to deliver results for tasks that may involve everything from code-first EF migrations to setting margins in CSS.
</p>
<p>
    The specification was intentionally brief and open ended to see what I would do with the freedom. Yet, it had some specifics that suggested certain goals
    and I played along. These are:
</p>
<p>
    · There are two mutually exclusive user roles, ‘Seller’ and ‘Buyer’. I believe the goal was to see how I would go about role based security and if I fall
    into any of the pitfalls of implementing one (which I hopefully didn’t, but do let me know). I did bend the rules a bit though, because I implemented only
    a single user view and limited visibility and behaviour there, with the potential to introduce a more fine grained (claims-based) access later on.
</p>
<p>
    · The features requested are minimalistic and the related user stories are incomplete. I did not aim to complement these; I just assumed these would get
    implemented in the next sprint as it happens sometimes.
</p>
<p>
    So, do not expect a website that could work sustainably. What happens to bids that were rejected/accepted? They stay, indefinitely. Can things be ordered?
    Not really. Lists grow large and ugly if you test a lot.
</p>
<p>
    At the same time, I did implement fancy stuff. Upload of multiple images simultaneously and ordering them with drag and drop? Check.
</p>
<h2>
    Implementation notes
</h2>
<p>
    The solution consists of two deliverables. An AngularJS based SPA and an OAuth Authorization Service. The SPA uses CORS to access the functionality of the
    Authorization Service, both in terms of the WebApi to register users and the token endpoint it uses to authenticate and pre-authorize users and retrieve a
    JWT token.
</p>
<p>
    For simplicity, I used a virtual audience – deep-purple-api – that both the Authorization Service API and the API layer implemented in the SPA’s host
    accept as evidence of authorization to use some of their services.
</p>
<h3>
    The Authorization Service
</h3>
<p>
    It is located in the deeP.AuthorizationService project.
</p>
<p>
    I think implementing a federated claims-based solution is definitely the right way to go when compared to a hybrid SPA (one that links out for cookie-based
    authentication). It’s better separation of concerns, potential for SSO across multiple services, easier standardization across different types of clients
    consuming the same services, and most importantly, it’s just more secure.
</p>
<p>
    However, do not expect a feature rich STS, it is purpose built for this sample. It uses symmetric keys for simple configuration and only has the Resource
    Owner flow implemented with password grant. I had limited time to implement miscellaneous infrastructure, so it shows my ‘just make it work’ code quality.
</p>
<p>
    The Authorization Service is built based on ASP.NET Identity user and role managers and its database is consequently code-first (which I created and
    committed into the repo for a more closely “F5 to run in Debug” experience).
    <br/>
    The ASP.NET Identity setup is minimalistic. I decided not to implement email validation, because reliable delivery of confirmation emails would require a
    custom domain with DKIM and SPF and not having these could have hindered acceptance testing.
</p>
<p>
    CORS policy: *
</p>
<h3>
    The SPA and its infrastructure
</h3>
<p>
    The remaining 4 projects belong to the SPA.
</p>
<p>
    deeP.Abstraction project: it’s quite shallow, actually. It consist of model classes and just two repository service interfaces.
</p>
<p>
    The model classes serve a single purpose: store domain object properties just long enough to get from the WebApi to the repositories. They get instantiated
    during deserialization of request bodies and become irrelevant once they are dispatched to a repository implementation – the first call, figuratively. They
    have no behaviour whatsoever. They do serve an important role however: they allow for abstracting data storage, -retrieval and -consistency away from the
    service layer and the data annotations on them are used by WebApi to pre-validate the model state.
</p>
<p>
    I decided to implement a SQL based repository implementation, because I haven’t used RavenDB before. It sounds good though, but it’s not the right time to
    learn. This is unfortunate, because considering the massive success Deep Purple will be and the lack of need for complex ACID transactions, a noSQL
    approach would have been more fitting.
</p>
<p>
    deeP.Repositories.* projects: I implemented the repositories together with the MS Test Unit Test project with a similar name. There are approximately 40
    tests with an emphasis on DML repository methods. There are two repositories.
</p>
<p>
    The PropertyRepository implements the create-update parts of a CRUD regarding the Property and Bid model objects. It also implements querying methods;
    normally these should be implemented using vanilla SQL, but I stuck with EF for rapid development. The repository wraps EF related exceptions and maps them
    into an implementation agnostic error code, allowing for a smoother replacement of the storage mechanism as the solution grows. The test set would need to
    be duplicated though, because EF is used there to validate if the DML using methods did in fact make the changes.
</p>
<p>
    The other repository is ImageRepository. It’s also based on SQL; the only sensible reason for this is that I wanted a self-contained solution without
    references to an external blob store (e.g. Azure blobs) or storing files locally, which would have eliminated horizontal scaling possibilities altogether.
</p>
<p>
    deeP.SPAWeb project: it’s an MVC5 project I started out from my favourite template. It’s a secure-by-default template that is also configured for
    performance and comes with a lot of default infrastructure. I normally adjust these default implementations substantially, however this time I left them
    mostly intact, just focusing on customizing the configurations to our needs (e.g. routes, CORS, CSP and CDN and bundle configurations). The two major
    additions I made: I added the WebApi and configured it for accepting JWT tokens and added AngularJS and the rest of the NuGet packages needed for the SPA.
</p>
<p>
    The api controllers are located in the Api folder, there are two of them, one for image-handling and another for properties. These are co-located now,
    however, implementing them in separate hosts might make sense as they have very different network and throttling characteristics – the image service
    handling fewer but larger requests and responses, while the other on the contrary. They should definitely be stored away from the MVC middleware as well,
    since we implemented an SPA and must not expect too high demand for serving the MVC content and this makes it rather expensive to initialize and run this
    infrastructure when we need to scale for the sake of the api services.
</p>
<p>
    The SPA is located in the Scripts/deeP folder. It is split into a number of Angular modules bound together by the app module ‘deeP’, initialized in the
    deeP-init.js file (no pun intended). The AppViews folder contains the root controllers of the views, the Authorization folder contains login and access
    controller related services and directives, while the Properties folder contains much of the presentation logic of the implementation.
    <br/>
    Most of them are trivial, with the exception of the editPropertyController, which controls the dialog used to add and edit properties. It contains a lot of
    code and not the best either, because it contains Google Map directives that need an awful lot of maintenance when shown on a modal.
</p>
<p>
    Generally speaking, I prefer introducing a view when the functionality or content makes it adequate to have deep link for it. The rest of the functionality
    (whether modal or directive based approach) depends in the context and is largely dependent on the user experience we want to achieve. I recommend to just
    follow the AppView controllers and the templates configured for them in the routing, and then the rest should unfold itself.
</p>
<p>
I believe the only view of note is the ‘user’ view. It’s a single view for all user functionality, regardless of user claims. We use the    <em>ngIfInRole</em> directive from Authorization that I created based on <em>ngIf</em> to limit the visibility of data and the execution of certain
    controllers and data access. The <em>accountContext</em> is a value shared across the modules that depend on <em>deePAuth</em>; it contains information
    about the user and his/her roles. Note however, that there is security-in-depth: the Api actions and repositories both add their validations on some level;
    the Api for access, the repository for consistency (e.g. a Seller cannot reopen bids, Buyer cannot change price once accepted, etc. – follow the unit
    tests).
    <br/>
    I decided not to use SignalR for bi-directional communication, for example, push notification of bid closure or new bid. This would have been overkill,
    plus it is not that kind of ‘low latency use case’.
</p>
<p>
    The <em>accountService</em> is used to register or log in a user (and thus manipulate the context). The interceptor is there to augment XHR with the bearer
    token on the fly or request redirection to login on 401. This is a fairly naïve approach, a renewal process should be in place based on refresh tokens or
    sliding windows. Note: a $watch captures changes to the <em>accountContext</em> and stores them in the local storage thus as long as the token is valid,
    the user should be able to leave the SPA and return without having to log in again, but it is only for a very short interval.
</p>
<p>
    You can find some Jasmine and angular-mock based test cases in the Script/Tests folder.
</p>
<p>
    deep-init.js contains the setup of the routing provider. I added a level of client-side access-control there, by resolving a potentially redirecting view
    state before each route change. It also works the other way around: if the user has an <em>accountContext</em>, he is automatically redirected to the
    user’s home view instead of the SPA default view.
</p>
<p>
    All routes can be deep-linked based on the routing setup of the MVC backend and properly configuring HTML5 mode and the base tag.
</p>
<h2>
    Limitations
</h2>
<p>
    SEO: I respected meta tags and the general rules (e.g. alt/title/aria here and there/lowercase uris, path endings, etc.) but did not implement a secondary
    route in the MVC app for the querystring based syntax that crawlers might use to get around SPA uris. I also did not maintain robots.txt, sitemap.xml,
    opensearch.xml, search engine ping and the rest of the infrastructure that comes with the template.
</p>
<p>
    Caching configurations need a lot of work.
</p>
<p>
    The <em>ImageController</em> can only handle images smaller than 4 MB and of JPEG type. I do not resize images or keep track of their mime types.
</p>
<p>
Some of the scripts, all templates and images are served from server and not from a CDN. This also stands for the images served by the    <em>ImageController</em>. Perhaps the easiest way to add these would be to change the Uri references to these items to a CDN that supports pulling the
    originals from the server.
</p>
<p>
    There is no propery tracing and instrumentation added neither on server, nor on client-side. I already paid the price in time for in during the Azure
    deployment due to some differences between IIS Express and IIS. There’s Elmah and Glimpse added though when the project is run in Debug mode.
</p>
<h2>
    Last words
</h2>
<p>
    Sorry for any remaining bugs. I hope you like it!
</p>
