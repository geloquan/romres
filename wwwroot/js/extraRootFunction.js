function placeHeaderAnchor(anchor_name, anchor_onclick, header_div, anchor_container) {
    const anchor = document.createElement('a');
    anchor.textContent = anchor_name;
    anchor.classList.add('underline');
    anchor.onclick = function() {
        anchor_onclick;
    }
}
function getParentSlotId(slotTree, inputSlotId) {
    function traverse(node) {
      if (!node) {
        return null;
      }
  
      if (node.SlotId === inputSlotId) {
        return node.ParentSlotId;
      }
  
      for (const child of node.SecondLayerChildren) {
        const parent = traverse(child);
        if (parent !== null) {
          return parent;
        }
      }
      for (const child of node.ThirdLayerChildren) {
        const parent = traverse(child);
        if (parent !== null) {
          return parent;
        }
      }
  
      return null;
    }
  
    return traverse(slotTree);
  }