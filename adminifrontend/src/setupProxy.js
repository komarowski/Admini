const { createProxyMiddleware } = require('http-proxy-middleware');

module.exports = function (app) {
    const appProxy = createProxyMiddleware('/api', {
        target: 'https://localhost:7097',
        secure: false,
    });

    const appProxy2 = createProxyMiddleware('/notes', {
        target: 'https://localhost:7097',
        secure: false,
    });

    app.use(appProxy);
    app.use(appProxy2);
};

