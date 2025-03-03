﻿using System.Collections.Generic;
using System.Threading.Tasks;
using GoogleMapsComponents;
using GoogleMapsComponents.Maps;
using GoogleMapsComponents.Maps.Drawing;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ServerSideDemo.Pages
{
    public partial class DrawingManagerPage
    {
        private DrawingManager _drawingManager;
        private DrawingManagerOptions _managerOptions;
        private PolygonOptions _polygonOptions;
        private GoogleMap map1;
        private MapOptions mapOptions;

        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        protected override void OnInitialized()
        {
            mapOptions = new MapOptions()
            {
                Zoom = 16,
                Center = new LatLngLiteral()
                {
                    Lat = -31.74230723298461,
                    Lng = -60.494505564961386
                },
                MapTypeId = MapTypeId.Roadmap,
                ZoomControl = true,
                DisableDefaultUI = true
            };

            _polygonOptions = new PolygonOptions()
            {
                StrokeWeight = 1,
                FillOpacity = 0.45f,
                Draggable = true,
                Editable = true,
                FillColor = "#FF0000",
                StrokeColor = "#FF0000",

            };
        }

        private async Task OnAfterInitAsync()
        {
            List<OverlayType> overlayTypes = new List<OverlayType>();
            overlayTypes.Add(OverlayType.Polygon);
            overlayTypes.Add(OverlayType.Polyline);
            overlayTypes.Add(OverlayType.Marker);

            var drawingControlOptions = new DrawingControlOptions()
            {
                Position = ControlPosition.TopCenter,
                DrawingModes = overlayTypes
            };

            _managerOptions = new DrawingManagerOptions()
            {
                Map = map1.InteropObject,
                PolygonOptions = _polygonOptions,
                //DrawingMode = OverlayType.Polygon,
                DrawingControl = true,
                DrawingControlOptions = drawingControlOptions
            };

            _drawingManager = await DrawingManager.CreateAsync(JsRuntime, _managerOptions);

            //https://developers.google.com/maps/documentation/javascript/drawinglayer
            await _drawingManager.AddOverlayCompleteListener(async (overComplete) =>
            {
                if (overComplete.Type == OverlayType.Polygon)
                {
                    var poly = overComplete.Polygon;
                    var polyPath = await poly.GetPath();
                    await poly.SetOptions(new PolygonOptions()
                    {
                        FillColor = "blue",
                        Editable = false,
                        Draggable = false
                    });
                }

            });


        }

        private async Task ChangeDrawingModeToLine()
        {
            await _drawingManager.SetDrawingMode(OverlayType.Polyline);

        }

        private async Task StopDrawingMode()
        {
            await _drawingManager.SetDrawingMode(null);

        }

        private Task SetMap()
        {
            return _drawingManager.SetMap(_drawingManager.GetMap());
        }
    }
}