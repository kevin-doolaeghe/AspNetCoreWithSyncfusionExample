"use strict";
var __createBinding = (this && this.__createBinding) || (Object.create ? (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    Object.defineProperty(o, k2, { enumerable: true, get: function() { return m[k]; } });
}) : (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    o[k2] = m[k];
}));
var __exportStar = (this && this.__exportStar) || function(m, exports) {
    for (var p in m) if (p !== "default" && !exports.hasOwnProperty(p)) __createBinding(exports, m, p);
};
exports.__esModule = true;
var index = require("@syncfusion/ej2-grids");
index.Grid.Inject(index.Filter, index.Page, index.Selection, index.Sort, index.Group, index.Reorder, index.RowDD, index.DetailRow, index.Toolbar, index.Aggregate, index.Search, index.VirtualScroll, index.Edit, index.Resize, index.ExcelExport, index.PdfExport, index.CommandColumn, index.ContextMenu, index.Freeze, index.ColumnMenu, index.ColumnChooser, index.ForeignKey, index.InfiniteScroll, index.LazyLoadGroup);
__exportStar(require("@syncfusion/ej2-grids"), exports);
