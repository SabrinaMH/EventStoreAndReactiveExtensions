fromCategory('Project')
  .foreachStream()
  .when({
      $any: function (state, ev) {
          linkTo('ByProject', ev);
      }
  });