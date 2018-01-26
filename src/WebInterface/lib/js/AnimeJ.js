/** 
 * @fileoverview This is the complete {@link http://www.codeplex.com/animej AnimeJ}
 * library, an animation for DHTML designed to be lightweight and efficient.
 * This library has been developed for {@link http://www.toscana4u.net/ Toscana4U}.
 *
 * @author Antonio Cisternino cisterni@di.unipi.it (C) 2008, University of Pisa
 * @version 1.0
 */

/**
 * @private
 */
var __DependenciesManager;

/**
 * Displays a blue screen inspired to Windows well known blue screen. The modal
 * popup is closed when you click on it.
 * @param {String} msg Message to display (preceded by 'INTERNAL ERROR: ')
 */
function BlueScreen(msg) {
  if (document.body) {
    alert('onloaded');
    var d = document.createElement('div'), k;
    var values = { 'position': 'absolute', 'top': 0, 'left': 0, 'width': '100%',
      'height': '100%', 'background': 'Blue', 'color': 'White', 'fontFamily': 'Fixedsys, monospace' };
    for (k in values) d.style[k] = values[k];
    d.innerHTML = 'INTERNAL ERROR: '+msg;
    d.onclick = function () { document.body.removeChild(d); };
    document.body.appendChild(d);
  } else {
    // Hack! when in header you cannot access to DOM
    document.write('<div style="z-index: 1000; position: absolute; left: 0; top: 0; margin: 0; padding: 0; width: 100%; height: 100%; background: Blue; color: White; font-family: Fixedsys, monospace" onclick="this.parentNode.removeChild(this);">INTERNAL ERROR: '+msg+'</div>');
  }
  throw "INTERNAL ERROR: " + msg;
}

/**
 * This function should be called by scripts to register their loading.
 * @param {String} name Name of the script
 * @param {int} version Version of the script, should be an increasing integer number
 * @param {string} description Brief description used for display purpose
 */
function RegisterScript(name, version, description) {
  if (! __DependenciesManager) __DependenciesManager = {};
  __DependenciesManager[name + ", version=" + version] = description;
}

/**
 * This function is meant to check dependencies among a number of JavaScript
 * source files that share this dependency convention. If a dependency is violated
 * the script displays a blue screen informing which scripts are not available.
 * @param {Array} scripts The required scripts. Each element of the array can be either
 *        a string with the script name (registered using RegisterScript) or a pair
 *        { Name: '...', Version: 0 } if the least version number is specified.
 */
function Require(scripts) {
  var i, msg = null;
  for (i = 0; i < scripts.length; i++) {
    var el = scripts[i], k, strong = typeof(el) != "string";
    if (strong && !el.Name) {
      msg = "Invalid require statement!";
      BlueScreen(msg);
      return;
    }
    var toadd = strong ? el.Name + ' (version ' + el.Version + ')' : el;
    for (k in __DependenciesManager)
      if (!strong) {
        if (k.indexOf(el) == 0) {
          toadd = null;
          break;
        }
      } else {
        if (k.indexOf(el.Name) == 0) {
          var v = parseInt(k.substring(k.indexOf(", version=") + 10));
          if (el.Version && el.Version <= v) {
            toadd = null;
            break;
          }
        }
      }
    if (toadd)
      if (msg) msg += ", " + toadd;
      else msg = toadd;
  }
  
  if (msg) BlueScreen('Missing libraries are <br><br>' + msg);
}

RegisterScript('AnimeJ', 1, 'AnimeJ animation library');

/**
 * @private
 * @class Heap implementation, it is used to define a shared timer which is responsible
 * for orchestrating the animation. This class is considered to be private to the library.
 * @constructor
 */
function AnimeJHeap() {
  // This is used to close this in local functions.
  var obj = this;

  // Current position of the heap index.
  var pos = 0;

  // Function to compute the left node of the tree.
  var left = function(idx) {
    return (2 * (idx + 1) - 1);
  }

  // Function to compute the right node of the tree.
  var right = function (idx) {
    return (2 * (idx + 1));
  }

  // Function to compute the parent node of a node.
  var parent = function (idx) {
    return (Math.floor((idx + 1) / 2) - 1);
  }
  
  // Function to swap the content of two nodes.
  var swap = function (a, b) {
    var tmp = obj[a];
    obj[a] = obj[b];
    obj[b] = tmp;
  }
  
  // Function to percolate up a node in the heap.
  var percolateUp = function (p) {
	var par = parent(p);
	while ((p > 0) && (obj[p].Due < obj[par].Due)) {
		swap(p, par);
		p = par;
		par = parent(p);
	}
  }
  
  // Function to percolate down a node in the heap.
  var percolateDown = function (p) {
	var l = left(p), r = right(p);
	if (l < pos) {
		if (r < pos) {
			if (obj[l].Due > obj[r].Due) {
				swap(p, r);
				percolateDown(r);
			} else {
				swap(p, l);
				percolateDown(l);
			}
		} else {
			swap(p, l);
			percolateDown(l);
		}
	} else if (p != pos - 1) {
		swap(p, pos - 1);
		percolateUp(p);
	}
  }
  
  /**
   * The number of elements contained in the heap.
   * @type int
   */
  this.Count = function () { return pos; }

  /**
   * Insert a Task into the heap.
   * @param {AnimeJTask} el Task to be inserted.
   */  
  this.Insert = function (el) {
    this[pos] = el;
    percolateUp(pos++);
  }

  /**
   * Read the top of the heap. If the heap is empty null is returned.
   * @type AnimeJTask
   */  
  this.Top = function () {
	if (pos)
		return this[0];
	return null;
  }

  /**
   * Remove the top element from the heap and returns it. If the heap
   * is empty null is returned and the heap is left unchanged.
   * @type AnimeJTask
   */  
  this.Remove = function () {
	var ret = null;
	if (!pos) return ret;
	ret = this[0];

	percolateDown(0);
	this[pos--] = null;

	return ret;
  }
  
  /**
   * Remove a specific task from the heap.
   * @param {AnimeJTask} t The task to remove. 
   * @type AnimeJTask
   */
  this.RemoveTask = function (t) {
    var i, ret = -1;
    for (i = 0; i < pos; i++)
      if (this[i].Task == t) {
        ret = this[i].Due;
        percolateDown(i);
	    this[pos--] = null;
        break;
      }
    return ret;
  }
  
  /**
   * Debug function to convert the heap into a string.
   * @private
   * @type String
   */
  this.ToString = function () {
    var i, ret = "";
    for (i = 0; i < pos; i++)
      ret += " <i>" + i + ":</i> " + this[i].Due;
    return ret;
  }
}

/**
 * @private
 * @class This is the base class for all tasks that can be scheduled
 * by the library. It simply defines the expected structure of an object
 * that is handled by the scheduler.
 * The model chosen by the library is that of Finite State Automata (FSA):
 * each activity is an automata that is notified as time passes and of other
 * events.
 */
function AnimeJTask() {
  /**
   * Notify the task that the animation has been paused.
   */
  this.OnPause = function () {};
  /**
   * Notify the task that the animation has been resumed.
   */
  this.OnResume = function () {};
  /**
   * Notify the task that the animation has been stopped.
   */
  this.OnStop = function () {};
  /**
   * Perform a step of computation
   */
  this.DoAction = function (delay) {};
}

/**
 * Global variable to keep track of the current timer.
 */ 
var AnimeJTimerRunning = null;

/**
 * @private
 * @class This class implements a shared timer using a heap and the
 * setTimeout javascript function. Usually this class is not used by
 * the public API and it is used by the library.
 */
function AnimeJTimer() {
  /**
   * @private
   * When the timer is started, it is used to record time.
   */
  this.Start = new Date();

  /**
   * @private
   * Heap used to store tasks ordered by due time.
   */
  this.Heap = new AnimeJHeap();

  /**
   * Return the timer time, a number from start time to
   * the current time.
   * @type int
   */  
  this.Time = function () {
    return new Date() - this.Start;
  }

  /**
   * Schedules a task for execution.
   * @param {AnimeJTask} what Task to be executed
   * @param {int} when Number of milliseconds to schedule the task.
   */
  this.SetAlertMillis = function(what, when) {
    var exp = when + this.Time();
    this.Heap.Insert({ 'Due': exp, 'Task': what });
    if (AnimeJTimerRunning != null && this.Heap.Top().Due == exp) {
      clearTimeout(AnimeJTimerRunning);
      AnimeJTimerRunning = null;
    }
    if (AnimeJTimerRunning == null) {
      AnimeJTimerRunning = true;
      var tosleep = this.Heap.Top().Due - this.Time();
      tosleep = 0 > tosleep ? 1 : tosleep;
      AnimeJTimerRunning = setTimeout('AnimeJTimerTick()', tosleep);
    }
  }
  
  /**
   * Remove a task from the scheduler. If the task is not present it does nothing.
   * @param {AnimeJTask} t Task to be removed.
   */ 
  this.RemoveTask = function (t) {
    var v = this.Heap.RemoveTask(t);
    if (v > -1) {
      v = v - this.Time();
      if (v < 0) v = 0;
    }
    return v;
  }
  
  /**
   * Schedules a task for execution.
   * @param {AnimeJTask} what Task to be executed
   * @param {int} when Absolute time at which the task should be scheduled.
   */
  this.SetAlertDate = function (what, when) {
    var t = when - this.Start;
    this.SetAlertMillis(what, t);
  }
}

/**
 * Global timer used by the library for all the animations.
 */
var AnimeJGlobalTimer = new AnimeJTimer();

/**
 * @private
 * Helper function that is used by the setTimeout function to invoke
 * the timer.
 */
function AnimeJTimerTick() {
  var t = AnimeJGlobalTimer;
  var currt = t.Time() + 10;
  while (t.Heap.Count() > 0 && currt >= t.Heap.Top().Due) {
    var el = t.Heap.Remove();
    el.Task.DoAction(currt - el.Due);
  }
  if (t.Heap.Count() > 0) {
    currt = t.Time();
    AnimeJTimerRunning = setTimeout('AnimeJTimerTick()', t.Heap.Top().Due - currt);
  } else {
    AnimeJTimerRunning = null;
  }
}

/**
 * @private
 * @class Conversion functions used by interpolators. They interpolate
 * a range of values using a parameter between 0.0 and 1.0 which
 * represents a time fraction.
 */
function AnimeJConv() {}

/**
 * Interpolates a single integer value
 * @param {int} from Starting value
 * @param {int} to Ending value
 * @param {float} v A fraction between 0.0 and 1.0
 * @type int
 * @final
 */
AnimeJConv.Int = function (from, to, v) {
    return Math.floor(from + (to - from)*v);
  };

/**
 * Interpolates a single float value
 * @param {float} from Starting value
 * @param {float} to Ending value
 * @param {float} v A fraction between 0.0 and 1.0
 * @type float
 * @final
 */
AnimeJConv.Float = function (from, to, v) {
    return from + (to - from)*v;
  };

/**
 * Interpolates between two objects in a discrete fashion: if v is
 * less than 0.5 the first is returned the second otherwise.
 * @param {Object} from Starting value
 * @param {Object} to Ending value
 * @param {float} v A fraction between 0.0 and 1.0
 * @type Object
 * @final
 */
AnimeJConv.Discrete = function (from, to, v) {
    return v < 0.5 ? from : to;
  };
  
/**
 * Interpolates an array of integers
 * @param {Array} from Starting value
 * @param {Array} to Ending value
 * @param {float} v A fraction between 0.0 and 1.0
 * @type Array
 * @final
 */
AnimeJConv.IntList = function (from, to, v) {
    var ret = new Array(), i;
    for (i = 0; i < from.length; i++)
      ret.push(Math.floor(from[i] + (to[i] - from[i])*v));
    return ret;
  };

/**
 * Interpolates an array of floats
 * @param {Array} from Starting value
 * @param {Array} to Ending value
 * @param {float} v A fraction between 0.0 and 1.0
 * @type Array
 * @final
 */
AnimeJConv.FloatList = function (from, to, v) {
    var ret = new Array(), i;
    for (i = 0; i < from.length; i++)
      ret.push(from[i] + (to[i] - from[i])*v);
    return ret;
  };


/**
 * @private
 * @class Generalized linear interpolator used to interpolate
 * the argument of a function or an object property like a style.
 * The interpolation is linear but it is possible to specify an array of
 * linear segments unevenly distributed in the interval [0.0, 1.0] which
 * is the range of time (normalized). For instance the following is a way to
 * distribute interpolation for a value from 0 to 1000 in a way that the first
 * half the value changes linearly in the interval 0-50 and in the second half
 * in the interval 50-1000:<br/><br/>
 * [ { t: 0.0, v: 0 }, { t: 0.5, v: 50 }, { t: 1.0, v: 1000 } ]
 * @constructor
 * @param {Object} obj Either an object (like a style of a DOM node) or a function
 * with a single parameter which gets invoked for each change of the value.
 * @param {String} prop The property name to be changed, it is considered only in case
 * the obj parameter refers to an object.
 * @param {Array} steps Array of pairs { t: t, v: v } ORDERED by the field t. The
 * t field is a time value and should be in the interval [0.0, 1.0] and the first
 * and the last values for t MUST be 0.0 and 1.0 respectively. The field v defines the
 * value that should be set when time reaches the given value. Interpolation of values
 * is performed 
 * @param {Function} conv Conversion function to interpolate two values given a time
 * value in the range [0.0, 1.0] like those defined in AnimeJConv
 * @param {String} prefix Prefix string for the output value.
 * @param {String} suffix Suffix string for the output value (for instance 'px').
 * @exception If steps.length is less than two, or the sequence of steps is unordered
 * with respect to time, or does not begin by 0.0 nor end with 1.0.
 * @see AnimeJConv
 */
function AnimeJLinearInterpolator(obj, prop, steps, conv, prefix, suffix) {
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

  /**
   * Computes a value given a time value in [0.0, 1.0]
   * @param {float} v Time value in the interval [0.0, 1.0]
   */
  this.Compute = function (v) {
    var idx;
    for (idx = 1; idx < steps.length; idx++)
      if (steps[idx].t >= v) break;
    
    idx--;
    var val = conv(steps[idx].v, steps[idx + 1].v, (v - steps[idx].t)/(steps[idx + 1].t - steps[idx].t));
    if (typeof(obj) == 'function')
      obj(val);
    else
      obj[prop] = prefix + val + suffix;
  }
}

/**
 * @private
 * @class Task for scheduling interpolators on the timer library.
 * The task is not trivial because it supports suspension and
 * resuming of tasks that means changing the absolute deadlines
 * inside the heap.
 * @constructor
 * @param {AnimeJLinearInterpolator} interp Interpolator to be used,
 * it can be an object with the Compute(v) function.
 * @param {int} ms Duration of the transition between 0.0 and 1.0 (and
 * consequently of the associated values).
 * @param {int} stepms The number of milliseconds between each value transition
 * (i.e. how frequent the value is changed).
 * @base AnimeJTask
 * @see AnimeJLinearInterpolator
 * @see AnimeJTimer
 */
function AnimeJInterpolatedTask(interp, ms, stepms) {
  /**
   * Duration of the whole transition in milliseconds.
   * @type {int}
   */
  this.Duration = ms;
  /**
   * Offset used to take into account suspension of task execution.
   * @type {int}
   */
  this.Start = - 1;

  /**
   * Invoked when the task is suspended, updates the internal state.
   */  
  this.OnPause = function() {
    if (this.Start != -1)
      this.Start = AnimeJGlobalTimer.Time() - this.Start;
  }
  
  /**
   * Invoked when the task is resumed, updates the internal state.
   */  
  this.OnResume = function() {
    if (this.Start != -1)
      this.Start = AnimeJGlobalTimer.Time() - this.Start;
  }
  
  /**
   * Invoked when the task is stopped, updates the internal state.
   */  
  this.OnStop = function() {
    this.Start = -1;
  }
    
  /**
   * It performs a step of animation by invoking the interpolator.
   * @param {int} d Delay in the notification with respect to the original deadline set.
   */  
  this.DoAction = function (d) {
    if (this.Start == -1)
      this.Start = AnimeJGlobalTimer.Time();
    
    var t = AnimeJGlobalTimer.Time();
    var v = (t - this.Start) / this.Duration;
    v = v > 1 ? 1 : v;
    interp.Compute(v);
    var t1 = AnimeJGlobalTimer.Time();
    var next = stepms - d + t1 - t; 
    if (v < 1)
      AnimeJGlobalTimer.SetAlertMillis(this, next);
    else
      this.Start = -1;
  }
}

/**
 * @private
 * @class Task used by the timeline to fire callbacks at the end of a timeline execution.
 * @constructor
 * @param {Timeline} t Timeline to be used to callback.
 * @param {String} prop Name of the property of the timeline holding the callback function.
 * @base AnimeJTask
 * @see AnimeJLinearInterpolator
 * @see AnimeJTimer
 */
function AnimeJTimelineCallBackTask(t, prop) {
  /**
   * Invoked when the task is paused, it does nothing.
   */  
  this.OnPause = function () {};

  /**
   * Invoked when the task is resumed, it does nothing.
   */  
  this.OnResume = function () {};

  /**
   * Invoked when the task is stopped, it does nothing.
   */  
  this.OnStop = function () {};

  /**
   * It invokes the specified callback on the property prop of the timeline t if defined.
   * @param {int} d Delay in the notification with respect to the original deadline set.
   */  
  this.DoAction = function (d) {
    if (t[prop])
      (t[prop])(d);
  }
}

/**
 * @private
 * @class Task used to invoke arbitrary functions when the task gets executed. The task is
 * fired only once.
 * @constructor
 * @param {Function} fun Function to be invoked when the task is executed, it may accept an integer
 * argument informing the delay of the execution with the expected deadline.
 * @base AnimeJTask
 * @see AnimeJLinearInterpolator
 * @see AnimeJTimer
 */
function AnimeJFunctionCallbackTask(fun) {
  /**
   * Invoked when the task is paused, it does nothing.
   */  
  this.OnPause = function () {};

  /**
   * Invoked when the task is resumed, it does nothing.
   */  
  this.OnResume = function () {};

  /**
   * Invoked when the task is stopped, it does nothing.
   */  
  this.OnStop = function () {};

  /**
   * It performs a step of animation by invoking the interpolator.
   * @param {int} d Delay in the notification with respect to the original deadline set.
   */  
  this.DoAction = function (d) {
    fun(d);
  }
}


/**
 * @class The timeline is the main interface to access the services of the animation library.
 * It features the classic interface to timeline in which tasks are scheduled at a given time
 * relative to the start of the execution. The SetAt method is meant for this purpose, and
 * an AnimeJTask should be provided. Usually the task is generated by one of the functions in
 * the AnimeJInterp class.<br/>
 * In the following example we have a text box that can be collapsed to left and vice versa.
 * This is the complete source code to show how expressive the library is:
 * <pre>
 * &lt;html&gt;
 * &lt;head&gt;
 * &lt;title&gt;Auto-hide text box&lt;/title&gt;
 * &lt;script type="text/javascript" src="..\..\src\AnimeJ.js"&gt;&lt;/script&gt;
 * &lt;script&gt;
 * function transition(btn) {
 *   var txt = btn.parentNode.childNodes[0];
 *   var t = new Timeline();
 *   if (txt.style.display == 'none') {
 *     txt.style.display = 'inline';
 *     t.SetAt(0, AnimeJInterp.px(1000, 30, txt.style, 'width', 0, 120));
 *     t.SetAt(0, AnimeJInterp.alpha(1000, 30, txt.style, 0.0, 1.0));
 *     t.OnEnd = function () { btn.innerHTML = '&amp;lt;&amp;lt;'; };
 *   } else {
 *     t.SetAt(0, AnimeJInterp.px(1000, 30, txt.style, 'width', 120, 0));
 *     t.SetAt(0, AnimeJInterp.alpha(1000, 30, txt.style, 1.0, 0.0));
 *     t.OnEnd = function (d) { btn.innerHTML = '&amp;gt;&amp;gt;'; txt.style.display = 'none'; };
 *   }
 *   t.Run();
 * }
 * &lt;/script&gt;
 * &lt;/head&gt;
 * &lt;body&gt;
 *   &lt;span&gt;
 *     &lt;input type="text" width="120px"&gt;&lt;/input&gt;
 *     &lt;span style="cursor: pointer; color: Blue" onclick="transition(this)"&gt;&amp;lt;&amp;lt;&lt;/span&gt;
 *   &lt;/span&gt;
 * &lt;/body&gt;
 * &lt;/html&gt;
 * </pre>
 * Note that the transition function simply prepares a timeline object t with a pixel interpolation
 * that changes the width property of the text box from 120 to 0 and vice versa. We also use fading by
 * changing the alpha during transition. Since the two transitions start at time 0 they run concurrently.
 * The OnEnd callback is used to update the text close to the text box.
 * @constructor
 * @see AnimeJInterp
 */
function Timeline() {
  var Paused = 0;
  var OnEndCB = null;
  var Data = {};
  
  /**
   * Tells wether the timeline is paused or not.
   * @type {bool}
   */
  this.IsPaused = function () {
    return Paused != 0;
  }

  /**
   * Executes the timeline.
   */  
  this.Run = function () {
    var max = 0;
    for (v in Data) {
      var el = Data[v];
      var startAt = parseInt(v);
      var i = 0;
      if (startAt > max) max = startAt;
      for (i = 0; i < el.length; i++) {
        if (el[i].Duration && (startAt + el[i].Duration) > max) max = startAt + el[i].Duration;
        AnimeJGlobalTimer.SetAlertMillis(el[i], startAt);
      }
    }
   var tm = this;
   OnEndCB = new AnimeJTimelineCallBackTask(tm, 'OnEnd');
   AnimeJGlobalTimer.SetAlertMillis(OnEndCB, max);
  }

  /**
   * Schedule a task for execution at a given time from the start of the timeline.
   * @param {int} timems Milliseconds to be elapsed since the beginning of execution
   * before executing the task anim.
   * @param {AnimeJTask} task Task to be executed after timems elapsed.
   */
  this.SetAt = function (timems, task) {
    if (Data[timems])
      Data[timems].push(task);
    else
      Data[timems] = new Array(task);
  }

  /**
   * Stops the execution of the timeline by removing all the tasks from the scheduler.
   * The timeline should be started by invoking Run().
   */
  this.Stop = function () {
    Paused = 0;
    var t = AnimeJGlobalTimer;
    var tm, i, v; 
    if (OnEndCB != null) {
      tm = t.RemoveTask(OnEndCB);
      OnEndCB = null;
    }
    
    for (v in Data) {
      var el = Data[v];
      for (i = 0; i < el.length; i++) {
        tm = t.RemoveTask(el[i]);
        el[i].OnStop();
      }
    }
  }


  /**
   * Pauses the execution of the current timeline. It can be resumed later on by invoking
   * the Resume() method.
   */  
  this.Pause = function () {
    if (Paused) return;
    Paused = 1;
    var t = AnimeJGlobalTimer;
    var tm, i, v; 
    if (OnEndCB != null) {
      tm = t.RemoveTask(OnEndCB);
      if (tm > -1)
        OnEndCB.Paused = tm;
      else
        OnEndCB = null;
    }
    
    for (v in Data) {
      var el = Data[v];
      for (i = 0; i < el.length; i++) {
        tm = t.RemoveTask(el[i]);
        if (tm == 0) tm = 1;// Hack: Paused == 0 is overloaded.
        el[i].OnPause();
        if (tm > -1)
          el[i].Paused = tm;
      }
    }
  }

  /**
   * Resumes the execution of a paused timeline.
   * @exception An exception is raised if the timeline is not paused.
   */
  this.Resume = function () {
    if (!Paused) throw "Timeline not paused!";
    Paused = 0;
    var t = AnimeJGlobalTimer;
    var tm, i, v; 
    if (OnEndCB != null) {
      t.SetAlertMillis(OnEndCB, OnEndCB.Paused);
      OnEndCB.Paused = 0;
    }
    
    for (v in Data) {
      var el = Data[v];
      for (i = 0; i < el.length; i++) {
        if (el[i].Paused) {
          t.SetAlertMillis(el[i], el[i].Paused);
          el[i].Paused = 0;
        }
        el[i].OnResume();
      }
    }
  }  
}

/**
 * Browser test. Used for setting alpha, but it can be useful in many other situations...
 */
var AnimeIsIE = navigator.appVersion.indexOf("MSIE") != -1;

/**
 * @class This class contains helpers for building interpolation tasks. It simply uses the
 * AnimeJLinearInterpolator in several different ways to ease common tasks. You can still
 * define your own interpolators. Apologize for interpolator names but they are conceived
 * to be short.
 */
function AnimeJInterp() {}

/**
 * Interpolates a pixel quantity (i.e. integer and appends px at the end).
 * @param {int} time Duration of the transition in milliseconds.
 * @param {int} tsteps Frequency (expressed in ms for an interval) at which
 * the property should be updated or the function called.
 * @param {Object} obj Object to set or function to invoke to set the value.
 * @param {Object} fromOrSteps If a number is given this is considered the starting value,
 * otherwise an array of steps is assumed as expected by AnimeJLinearInterpolator.
 * @param {int} to If specified (i.e. the fromOrSteps argument is an integer) it is
 * considered the target value for the interpolation.
 * @see AnimeJLinearInterpolator
 */
AnimeJInterp.px = function (time, tstep, obj, prop, fromOrSteps, to) {
    if (typeof(fromOrSteps) == 'number')
      fromOrSteps = [ { t: 0.0, v: fromOrSteps}, { t: 1.0, v: to } ];
    return new AnimeJInterpolatedTask(new AnimeJLinearInterpolator(obj, prop, fromOrSteps, AnimeJConv.Int, '', 'px'), time, tstep);
  };

  // Transparency from 0.0 to 1.0!
/**
 * Interpolates the alpha of an element in the appropriate way for the current browser.
 * @param {int} time Duration of the transition in milliseconds.
 * @param {int} tsteps Frequency (expressed in ms for an interval) at which
 * the property should be updated or the function called.
 * @param {Object} style The style object holding the alpha property to be changed.
 * @param {Object} fromOrSteps If a number between 0.0 (fully transparent) and 1.0
 * (totally opaque) is provided, this is considered the starting value;
 * otherwise an array of steps is assumed as expected by AnimeJLinearInterpolator.
 * @param {float} to If specified (i.e. the fromOrSteps argument is an integer) it is
 * considered the target value for the interpolation (should be a value in the range [0.0, 1.0].
 * @see AnimeJLinearInterpolator
 */
AnimeJInterp.alpha = function (time, tstep, style, fromOrSteps, to) {
    if (typeof(fromOrSteps) == 'number')
      fromOrSteps = [ { t: 0.0, v: fromOrSteps}, { t: 1.0, v: to } ];
    if (AnimeIsIE) {
      var i;
      for (i = 0; i < fromOrSteps.length; i++)
        fromOrSteps[i].v = Math.floor(fromOrSteps[i].v * 100);
      return new AnimeJInterpolatedTask(new AnimeJLinearInterpolator(style, 'filter', fromOrSteps, AnimeJConv.Int, 'alpha(opacity=', ')'), time, tstep);
    }
    
    return new AnimeJInterpolatedTask(new AnimeJLinearInterpolator(style, 'opacity', fromOrSteps, AnimeJConv.Float, '', ''), time, tstep);
  };
  
/**
 * Interpolates an integer value and pass it to a funciton (fiv: function integer value).
 * @param {int} time Duration of the transition in milliseconds.
 * @param {int} tsteps Frequency (expressed in ms for an interval) at which
 * the property should be updated or the function called.
 * @param {Function} fun Function to be invoked with the current value.
 * @param {Object} fromOrSteps If a number is given this is considered the starting value,
 * otherwise an array of steps is assumed as expected by AnimeJLinearInterpolator.
 * @param {int} to If specified (i.e. the fromOrSteps argument is an integer) it is
 * considered the target value for the interpolation.
 * @see AnimeJLinearInterpolator
 */
AnimeJInterp.fiv = function (time, tstep, fun, fromOrSteps, to) {
    if (to)
      fromOrSteps = [ { t: 0.0, v: fromOrSteps}, { t: 1.0, v: to } ];
    return new AnimeJInterpolatedTask(new AnimeJLinearInterpolator(fun, null, fromOrSteps, AnimeJConv.Int, '', ''), time, tstep);
  };

/**
 * Interpolates a float value and pass it to a funciton (ffv: function float value).
 * @param {int} time Duration of the transition in milliseconds.
 * @param {int} tsteps Frequency (expressed in ms for an interval) at which
 * the property should be updated or the function called.
 * @param {Function} fun Function to be invoked with the current value.
 * @param {Object} fromOrSteps If a number is given this is considered the starting value,
 * otherwise an array of steps is assumed as expected by AnimeJLinearInterpolator.
 * @param {float} to If specified (i.e. the fromOrSteps argument is a float) it is
 * considered the target value for the interpolation.
 * @see AnimeJLinearInterpolator
 */
AnimeJInterp.ffv = function (time, tstep, fun, fromOrSteps, to) {
    if (to)
      fromOrSteps = [ { t: 0.0, v: fromOrSteps}, { t: 1.0, v: to } ];
    return new AnimeJInterpolatedTask(new AnimeJLinearInterpolator(fun, null, fromOrSteps, AnimeJConv.Float, '', ''), time, tstep);
  };

/**
 * Interpolates an array of integers and pass it to a funciton (fia: function int array).
 * @param {int} time Duration of the transition in milliseconds.
 * @param {int} tsteps Frequency (expressed in ms for an interval) at which
 * the property should be updated or the function called.
 * @param {Function} fun Function to be invoked with the current value.
 * @param {Array} fromOrSteps If the to argument is given this is considered the starting value,
 * otherwise an array of steps is assumed as expected by AnimeJLinearInterpolator.
 * @param {Array} to If specified (i.e. the fromOrSteps argument is considered to be an array of int) it is
 * considered the target value for the interpolation.
 * @see AnimeJLinearInterpolator
 */
AnimeJInterp.fia = function (time, tstep, fun, fromOrSteps, to) {
    if (to)
      fromOrSteps = [ { t: 0.0, v: fromOrSteps}, { t: 1.0, v: to } ];
    return new AnimeJInterpolatedTask(new AnimeJLinearInterpolator(fun, null, fromOrSteps, AnimeJConv.IntList, '', ''), time, tstep);
  };

/**
 * Interpolates an array of floats and pass it to a funciton (ffa: function float array).
 * @param {int} time Duration of the transition in milliseconds.
 * @param {int} tsteps Frequency (expressed in ms for an interval) at which
 * the property should be updated or the function called.
 * @param {Function} fun Function to be invoked with the current value.
 * @param {Array} fromOrSteps If the to argument is given this is considered the starting value,
 * otherwise an array of steps is assumed as expected by AnimeJLinearInterpolator.
 * @param {Array} to If specified (i.e. the fromOrSteps argument is considered to be an array of float) it is
 * considered the target value for the interpolation.
 * @see AnimeJLinearInterpolator
 */
AnimeJInterp.ffa = function (time, tstep, fun, fromOrSteps, to) {
    if (to)
      fromOrSteps = [ { t: 0.0, v: fromOrSteps}, { t: 1.0, v: to } ];
    return new AnimeJInterpolatedTask(new AnimeJLinearInterpolator(fun, null, fromOrSteps, AnimeJConv.FloatList, '', ''), time, tstep);
  };
