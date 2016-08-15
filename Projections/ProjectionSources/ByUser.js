fromAll()
  .when({
      $any: function (state, ev) {
          if (ev.data.User) {
              linkTo(ev.data.User, ev);
          }
      }
  });