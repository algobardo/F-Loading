/////////////////////////////////////////////////////////////
//
// animate.js
//
// Antonio Cisternino 2008
//
// http://www.toscana4u.net
//
////////////////////////////////////////////////////////////

//
// Heap Implementation
//

function AnimeHeap(comp) {
  this.pos = 0;
  this.Comparer = comp;
  
  this.Count = function () { return this.pos; }

  this.Left = function(idx) {
    return (2 * (idx + 1) - 1);
  }

  this.Right = function (idx) {
    return (2 * (idx + 1));
  }

  this.Parent = function (idx) {
    return (Math.floor((idx + 1) / 2) - 1);
  }
  
  this.Swap = function (a, b) {
    var tmp = this[a];
    this[a] = this[b];
    this[b] = tmp;
  }
  
  this.PercolateUp = function (pos) {
	var par = this.Parent(pos);
	while ((pos > 0) && (this.Comparer(this[pos], this[par]) < 0)) {
		this.Swap(pos, par);
		pos = par;
		par = this.Parent(pos);
	}
  }
  
  this.PercolateDown = function (pos) {
	var l = this.Left(pos), r = this.Right(pos);
	if (l < this.pos) {
		if (r < this.pos) {
			if (this.Comparer(this[l], this[r]) > 0) {
				this.Swap(pos, r);
				this.PercolateDown(r);
			} else {
				this.Swap(pos, l);
				this.PercolateDown(l);
			}
		} else {
			this.Swap(pos, l);
			this.PercolateDown(l);
		}
	} else if (pos != this.pos - 1) {
		this.Swap(pos, this.pos - 1);
		this.PercolateUp(pos);
	}
  }
  
  this.Insert = function (el) {
    this[this.pos] = el;
    this.PercolateUp(this.pos++);
  }
  
  this.Top = function () {
	if (this.pos)
		return this[0];
	return null;
  }
  
  this.Remove = function () {
	var ret = null;
	if (!this.pos) return ret;
	ret = this[0];

	this.PercolateDown(0);
	this[this.pos--] = null;

	return ret;
  }
  
  this.RemoveTask = function (t) {
    var i, ret = -1;
    for (i = 0; i < this.pos; i++)
      if (this[i].Task == t) {
        ret = this[i].Due;
        this.PercolateDown(i);
	    this[this.pos--] = null;
        break;
      }
    return ret;
  }
  
  this.ToString = function () {
    var i, ret = "";
    for (i = 0; i < this.pos; i++)
      ret += " <i>" + i + ":</i> " + this[i].Due;
    return ret;
  }
}

//
// "Base class" for tasks
//
function AnimeAnimation() {
  this.Pause = function () {};
  this.Resume = function () {};
  this.Stop = function () {};
  this.DoAction = function (delay) {};
}

//
// Timer
// 
var AnimeTimerRunning = null;

function AnimeTimer() {
  this.Start = new Date();
  this.Heap = new AnimeHeap(function (a,b) { return a.Due - b.Due; });
  
  this.Time = function () {
    return new Date() - this.Start;
  }

  this.SetAlertMillis = function(what, when) {
    var exp = when + this.Time();
    this.Heap.Insert({ 'Due': exp, 'Task': what });
    if (AnimeTimerRunning != null && this.Heap.Top().Due == exp) {
      clearTimeout(AnimeTimerRunning);
      AnimeTimerRunning = null;
    }
    if (AnimeTimerRunning == null) {
      AnimeTimerRunning = true;
      var tosleep = this.Heap.Top().Due - this.Time();
      tosleep = 0 > tosleep ? 1 : tosleep;
      AnimeTimerRunning = setTimeout('AnimeTimerTick()', tosleep);
    }
  }
    
  this.RemoveTask = function (t) {
    var v = this.Heap.RemoveTask(t);
    if (v > -1) {
      v = v - this.Time();
      if (v < 0) v = 0;
    }
    return v;
  }
  
  this.SetAlertDate = function (what, when) {
    var t = when - this.Time();
    this.SetAlertMillis(what, t);
  }
}

var AnimeGlobalTimer = new AnimeTimer();

function AnimeTimerTick() {
  var t = AnimeGlobalTimer;
  var currt = t.Time() + 10;
  while (t.Heap.Count() > 0 && currt >= t.Heap.Top().Due) {
    var el = t.Heap.Remove();
    el.Task.DoAction(currt - el.Due);
  }
  if (t.Heap.Count() > 0) {
    currt = t.Time();
    AnimeTimerRunning = setTimeout('AnimeTimerTick()', t.Heap.Top().Due - currt);
  } else {
    AnimeTimerRunning = null;
  }
}

// Value Interpolators
//
function AnimeLinearIntegerInterpolator(obj, prop, from, to, prefix, suffix) {
  this.Obj = obj;
  this.Prop = prop;
  this.From = from;
  this.To = to;
  this.Suffix = suffix;
  this.Prefix = prefix;
  
  // v is between 0.0 and 1.0
  this.Compute = function (v) {
    var val = this.From + (this.To - this.From)*v;
    this.Obj[this.Prop] = this.Prefix + val + this.Suffix;
  }
}


var AnimeConv = {
  Int: function (from, to, v) {
    return Math.floor(from + (to - from)*v);
  },

  Float: function (from, to, v) {
    return from + (to - from)*v;
  },

  Discrete: function (from, to, v) {
    return v < 0.5 ? from : to;
  }
}



// Steps is an array of pairs { t: t, v: v }
// The algorithm ASSUMES the array is ordered in time values
// t are all different and values must be in the interval [0.0, 1.0]
// and the first and the last values must be 0.0 and 1.0 respectively
// Conv should be a function that maps interval [0.0, 1.0] into from to,
// signature is conv(from, to, v);
//
// Example: [ { t: 0.0, v: 0 }, { t: 0.1, v: 50 }, { t: 1.0, v: 1000 } ]
function AnimeLinearInterpolator(obj, prop, steps, conv, prefix, suffix) {
  var i, last = 0.0;
  // typecheck steps
  if (steps.length < 2) throw "Invalid steps: length less than 2";
  if (steps[0].t != 0.0) throw "Invalid steps: timeline must start at 0.0!";
  if (steps[steps.length - 1].t != 1.0) throw "Invalid steps: timeline must end with 1.0!";
  for (i = 1; i < steps.length; i++) {
    if (steps[i].t > 1.0 || steps[i].t < 0.0 || steps[i].t <= last)
      throw "Invalid steps: invalid value "+(steps[i].t) +"!";
    last = steps[i].t;
  }

  this.Obj = obj;
  this.Prop = prop;
  this.Steps = steps;
  this.Suffix = suffix;
  this.Prefix = prefix;
  this.Conversion = conv;
  
  // v is between 0.0 and 1.0
  this.Compute = function (v) {
    var idx;
    var steps = this.Steps;
    for (idx = 1; idx < steps.length; idx++)
      if (steps[idx].t >= v) break;
    
    idx--;
    var val = this.Conversion(steps[idx].v, steps[idx + 1].v, (v - steps[idx].t)/(steps[idx + 1].t - steps[idx].t));
    this.Obj[this.Prop] = this.Prefix + val + this.Suffix;
  }
}

//
// Animate interpolators
//

function AnimeAnimate(interp, ms, stepms) {
  this.Duration = ms;
  this.Start = - 1;
  this.Step = stepms;
  this.Interp = interp;
  
  this.Pause = function() {
    if (this.Start != -1)
      this.Start = AnimeGlobalTimer.Time() - this.Start;
  }
  
  this.Resume = function() {
    if (this.Start != -1)
      this.Start = AnimeGlobalTimer.Time() - this.Start;
  }
  
  this.Stop = function() {
    this.Start = -1;
  }
    
  this.DoAction = function (d) {
    if (this.Start == -1)
      this.Start = AnimeGlobalTimer.Time();
    
    var t = AnimeGlobalTimer.Time();
    var v = (t - this.Start) / this.Duration;
    v = v > 1 ? 1 : v;
    this.Interp.Compute(v);
    var t1 = AnimeGlobalTimer.Time();
    var next = stepms - d + t1 - t; 
    if (v < 1)
      AnimeGlobalTimer.SetAlertMillis(this, next);
    else
      this.Start = -1;
  }
}

function AnimeCBackAnimation(t, prop) {
  this.Timeline = t;
  this.Pause = function () {};
  this.Resume = function () {};
  this.Stop = function () {};
  this.DoAction = function (d) {
    if (t[prop])
      (t[prop])(d);
  }
}


//
// Timeline
//

function Timeline() {
  this.Data = {};
  this.OnEndCB = null;
  this.Paused = 0;
  
  this.Run = function () {
    var max = 0;
    for (v in this.Data) {
      var el = this.Data[v];
      var startAt = parseInt(v);
      var i = 0;
      if (startAt > max) max = startAt;
      for (i = 0; i < el.length; i++) {
        if (el[i].Duration && (startAt + el[i].Duration) > max) max = startAt + el[i].Duration;
        AnimeGlobalTimer.SetAlertMillis(el[i], startAt);
      }
    }
   var tm = this;
   this.OnEndCB = new AnimeCBackAnimation(tm, 'OnEnd');
   AnimeGlobalTimer.SetAlertMillis(this.OnEndCB, max);
  }

  this.SetAt = function (timems, anim) {
    if (this.Data[timems])
      this.Data[timems].push(anim);
    else
      this.Data[timems] = new Array(anim);
  }

  this.Stop = function () {
    this.Paused = 0;
    var t = AnimeGlobalTimer;
    var tm, i, v; 
    if (this.OnEndCB != null) {
      tm = t.RemoveTask(this.OnEndCB);
      this.OnEndCB = null;
    }
    
    for (v in this.Data) {
      var el = this.Data[v];
      for (i = 0; i < el.length; i++) {
        tm = t.RemoveTask(el[i]);
        el[i].Stop();
      }
    }
  }

  
  this.Pause = function () {
    if (this.Paused) throw "Timeline already paused!";
    this.Paused = 1;
    var t = AnimeGlobalTimer;
    var tm, i, v; 
    if (this.OnEndCB != null) {
      tm = t.RemoveTask(this.OnEndCB);
      if (tm > -1)
        this.OnEndCB.Paused = tm;
      else
        this.OnEndCB = null;
    }
    
    for (v in this.Data) {
      var el = this.Data[v];
      for (i = 0; i < el.length; i++) {
        tm = t.RemoveTask(el[i]);
        if (tm == 0) tm = 1;// Hack: Paused == 0 is overloaded.
        el[i].Pause();
        if (tm > -1)
          el[i].Paused = tm;
      }
    }
  }

  this.Resume = function () {
    if (!this.Paused) throw "Timeline not paused!";
    this.Paused = 0;
    var t = AnimeGlobalTimer;
    var tm, i, v; 
    if (this.OnEndCB != null) {
      t.SetAlertMillis(this.OnEndCB, this.OnEndCB.Paused);
      this.OnEndCB.Paused = 0;
    }
    
    for (v in this.Data) {
      var el = this.Data[v];
      for (i = 0; i < el.length; i++) {
        if (el[i].Paused) {
          t.SetAlertMillis(el[i], el[i].Paused);
          el[i].Paused = 0;
        }
        el[i].Resume();
      }
    }
  }  
}

var AnimeIsIE = navigator.appVersion.indexOf("MSIE") != -1;

var AnimeInterp = {

  px: function (time, tstep, obj, prop, fromOrSteps, to) {
    if (typeof(fromOrSteps) == 'number')
      fromOrSteps = [ { t: 0.0, v: fromOrSteps}, { t: 1.0, v: to } ];
    return new AnimeAnimate(new AnimeLinearInterpolator(obj, prop, fromOrSteps, AnimeConv.Int, '', 'px'), time, tstep);
  },

  // Transparency from 0.0 to 1.0!
  alpha: function (time, tstep, style, fromOrSteps, to) {
    if (typeof(fromOrSteps) == 'number')
      fromOrSteps = [ { t: 0.0, v: fromOrSteps}, { t: 1.0, v: to } ];
    if (AnimeIsIE) {
      var i;
      for (i = 0; i < fromOrSteps.length; i++)
        fromOrSteps[i].v = Math.floor(fromOrSteps[i].v * 100);
      return new AnimeAnimate(new AnimeLinearInterpolator(style, 'filter', fromOrSteps, AnimeConv.Int, 'alpha(opacity=', ')'), time, tstep);
    }
    
    return new AnimeAnimate(new AnimeLinearInterpolator(style, 'opacity', fromOrSteps, AnimeConv.Float, '', ''), time, tstep);
  }

};
