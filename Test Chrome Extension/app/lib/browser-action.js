(function (h) {
  'use strict';

  // Cache the tab state constants.
  var states = h.TabState.states;

  // Each button state has two icons one for normal resolution (19) and one
  // for hi-res screens (38).
  var icons = {};
  icons[states.ACTIVE] = {
    19: 'images/notocol-19-active.png',
    38: 'images/notocol-38-active.png'
  };
  icons[states.INACTIVE] = {
    19: 'images/notocol-19-inactive.png',
    38: 'images/notocol-38-inactive.png'
  };

  // Fake localization function.
  function _(str) {
    return str;
  }

  /* Controls the display of the browser action button setting the icon, title
   * and badges depending on the current state of the tab. This is a stateless
   * module and does not store the current state. A TabState instance should
   * be used to manage which tabs are active/inactive.
   */
  function BrowserAction(chromeBrowserAction) {
    this.setState = function (tabId, state) {
      switch (state) {
        case states.ACTIVE:   this.activate(tabId); break;
        case states.INACTIVE: this.deactivate(tabId); break;
        case states.ERRORED:  this.error(tabId); break;
        default: throw new TypeError('State ' + state + ' is invalid');
      }
    };

    /* Sets the active browser action appearance for the provided tab id. */
    this.activate = function (tabId) {
      //chromeBrowserAction.setIcon({tabId: tabId, path: icons[states.ACTIVE]});
      //chromeBrowserAction.setTitle({tabId: tabId, title: _('Hypothesis is active')});
    };

    /* Sets the inactive browser action appearance for the provided tab id. */
    this.deactivate = function (tabId) {
      //chromeBrowserAction.setIcon({tabId: tabId, path: icons[states.INACTIVE]});
      //chromeBrowserAction.setTitle({tabId: tabId, title: _('Hypothesis is inactive')});
    };

    /* Sets the errored browser action appearance for the provided tab id. */
    this.error = function (tabId) {
      //chromeBrowserAction.setIcon({tabId: tabId, path: icons[states.INACTIVE]});
      chromeBrowserAction.setTitle({tabId: tabId, title: _('Notocol note taker has failed to load')});
      chromeBrowserAction.setBadgeText({tabId: tabId, text: '!'});
    };
  }

  BrowserAction.icons = icons;

  h.BrowserAction = BrowserAction;
})(window.h || (window.h = {}));
